using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Hangfire;
using System;
using System;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly EmailTemplateService _emailTemplateService;

    public IdentityService(
        UserManager<ApplicationUser> userManager, 
        SignInManager<ApplicationUser> signInManager, 
        IConfiguration configuration,
        IEmailService emailService,
        IBackgroundJobClient backgroundJobClient,
        EmailTemplateService emailTemplateService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _emailService = emailService;
        _backgroundJobClient = backgroundJobClient;
        _emailTemplateService = emailTemplateService;
    }

    public async Task<AuthResponseDto> RegisterAsync(string email, string password, string firstName, string lastName, string gender, string nationality, DateTime dateOfBirth, string? profileImage)
    {
        var userExists = await _userManager.FindByEmailAsync(email);
        if (userExists != null)
            throw new BadRequestException("User already exists!");

        var user = new ApplicationUser
        {
            Email = email,
            UserName = email,
            SecurityStamp = Guid.NewGuid().ToString(),
            FirstName = firstName,
            LastName = lastName,
            Gender = gender,
            Nationality = nationality,
            DateOfBirth = dateOfBirth,
            ProfileImage = profileImage,
            IsVerified = false
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            throw new BadRequestException("User creation failed! Errors: " + string.Join(", ", result.Errors.Select(e => e.Description)));

        // Generate OTP
        var otp = new Random().Next(100000, 999999).ToString();
        user.EmailVerificationCode = otp;
        user.EmailVerificationCodeExpiresAt = DateTime.UtcNow.AddMinutes(10);
        await _userManager.UpdateAsync(user);

        // Send OTP Email
        _backgroundJobClient.Enqueue(() => _emailService.SendEmailAsync(
            email, 
            "Verify Your Email", 
            $"Your verification code is {otp}",
            otp
        ));

        return await GenerateAuthResponseAsync(user);
    }

    public async Task<bool> ConfirmEmailAsync(string email, string code)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) throw new NotFoundException("User not found");

        if (user.IsVerified) return true;

        if (user.EmailVerificationCode != code || user.EmailVerificationCodeExpiresAt < DateTime.UtcNow)
        {
            throw new BadRequestException("Invalid or expired verification code");
        }

        user.IsVerified = true;
        user.EmailVerificationCode = null;
        user.EmailVerificationCodeExpiresAt = null;
        await _userManager.UpdateAsync(user);

        return true;
    }

    public async Task ResendOtpAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) throw new NotFoundException("User not found");

        if (user.IsVerified) throw new BadRequestException("Email is already verified");

        // Generate new OTP
        var otp = new Random().Next(100000, 999999).ToString();
        user.EmailVerificationCode = otp;
        user.EmailVerificationCodeExpiresAt = DateTime.UtcNow.AddMinutes(10);
        
        await _userManager.UpdateAsync(user);

        // Send OTP Email
        _backgroundJobClient.Enqueue(() => _emailService.SendEmailAsync(
            email, 
            "Verify Your Email - Resend", 
            $"Your new verification code is",
            otp
        ));
    }

    public async Task<AuthResponseDto> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) 
            throw new UnauthorizedException("Unauthorized: Invalid Credentials");

        if (await _userManager.IsLockedOutAsync(user))
            throw new UnauthorizedException($"Account is locked out. Try again in {user.LockoutEnd?.Subtract(DateTime.UtcNow).Minutes} minutes.");

        if (!await _userManager.CheckPasswordAsync(user, password))
        {
            await _userManager.AccessFailedAsync(user);
            if (await _userManager.IsLockedOutAsync(user))
             throw new UnauthorizedException("Account locked out due to multiple failed attempts.");
             
            throw new UnauthorizedException("Unauthorized: Invalid Credentials");
        }

        await _userManager.ResetAccessFailedCountAsync(user);

        return await GenerateAuthResponseAsync(user);
    }

    public async Task<AuthResponseDto> LoginWithGoogleAsync(string idToken)
    {
        var clientId = _configuration["GoogleAuth:ClientId"];
        var clientSecret = _configuration["GoogleAuth:ClientSecret"]; // Available if needed for Code Flow

        using var client = new HttpClient();
        var response = await client.GetAsync($"https://oauth2.googleapis.com/tokeninfo?id_token={idToken}");
        if (!response.IsSuccessStatusCode)
             throw new UnauthorizedException("Google token validation failed.");

        var content = await response.Content.ReadAsStringAsync();
        using var doc = System.Text.Json.JsonDocument.Parse(content);
        var payload = doc.RootElement;
        
        // Validate Audience matches ClientId
        var aud = payload.GetProperty("aud").GetString();
        if (aud != clientId)
        {
            throw new UnauthorizedException("Invalid Google Token Audience.");
        }

        string email = payload.GetProperty("email").GetString()!;
        if (string.IsNullOrEmpty(email)) throw new UnauthorizedException("Email not found in Google Token.");

        string firstName = payload.TryGetProperty("given_name", out var fn) ? fn.GetString() ?? "" : "";
        string lastName = payload.TryGetProperty("family_name", out var ln) ? ln.GetString() ?? "" : "";
        
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
             user = new ApplicationUser
            {
                Email = email,
                UserName = email,
                SecurityStamp = Guid.NewGuid().ToString(),
                FirstName = firstName,
                LastName = lastName,
                IsVerified = true,
                CreatedAt = DateTime.UtcNow
            };
            var result = await _userManager.CreateAsync(user); 
             if (!result.Succeeded) throw new BadRequestException("Google User creation failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }
        
        return await GenerateAuthResponseAsync(user);
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string token, string refreshToken)
    {
        var principal = GetPrincipalFromExpiredToken(token);
        if (principal == null) throw new UnauthorizedException("Invalid token");

        string username = principal.Identity!.Name!; // or ClaimTypes.Name / Email
        // If UserName is Email
        var user = await _userManager.FindByEmailAsync(username);

        if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new UnauthorizedException("Invalid access token or refresh token");
        }

        return await GenerateAuthResponseAsync(user);
    }

    public async Task ForgotPasswordAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return; 

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        
        // Construct Reset Link
        var clientUrl = _configuration["ClientAppUrl"] ?? "http://localhost:3000";
        var encodedToken = Uri.EscapeDataString(token);
        var encodedEmail = Uri.EscapeDataString(email);
        var resetLink = $"{clientUrl}/auth/reset-password?token={encodedToken}&email={encodedEmail}";

        string message = "We received a request to reset your password. Click the button below to proceed.";

        _backgroundJobClient.Enqueue(() => _emailService.SendEmailAsync(
            email, 
            "Reset Password", 
            message,
            "", // No OTP
            resetLink,
            "Reset Password"
        ));
    }

    public async Task ResetPasswordAsync(string email, string token, string newPassword)
    {
         var user = await _userManager.FindByEmailAsync(email);
         if (user == null) throw new NotFoundException("User not found");
         
         var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
         if (!result.Succeeded) throw new BadRequestException("Reset failed: " + string.Join(" ", result.Errors.Select(e => e.Description)));
    }
    
    public async Task ChangePasswordAsync(string email, string currentPassword, string newPassword)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) throw new NotFoundException("User not found");
        
        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
         if (!result.Succeeded) throw new BadRequestException("Change password failed: " + string.Join(" ", result.Errors.Select(e => e.Description)));
    }
    
    public async Task<bool> ValidateTokenAsync(string token)
    {
        // Simple handler, normally handled by Middleware
        var tokenHandler = new JwtSecurityTokenHandler();
        try 
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]!)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private async Task<AuthResponseDto> GenerateAuthResponseAsync(ApplicationUser user)
    {
        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var userRoles = await _userManager.GetRolesAsync(user);
        foreach (var role in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, role));
        }

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]!));

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            expires: DateTime.UtcNow.AddMinutes(double.Parse(_configuration["JwtSettings:DurationInMinutes"] ?? "60")),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        var refreshToken = GenerateRefreshToken();
        
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userManager.UpdateAsync(user);

        return new AuthResponseDto
        {
            Id = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            RefreshToken = refreshToken,
            RefreshTokenExpiration = user.RefreshTokenExpiryTime
        };
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false, // Set to true if you want to validate
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]!)),
            ValidateLifetime = false // In this case, we don't care about the token's expiration date
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return principal;
    }
}
