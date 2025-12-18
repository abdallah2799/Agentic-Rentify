using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;

    public IdentityService(
        UserManager<ApplicationUser> userManager, 
        SignInManager<ApplicationUser> signInManager, 
        IConfiguration configuration,
        IEmailService emailService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _emailService = emailService;
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

        return await GenerateAuthResponseAsync(user);
    }

    public async Task<AuthResponseDto> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, password))
            throw new UnauthorizedException("Unauthorized: Invalid Credentials");

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
        if (user == null) return; // Silent return for security

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        // In a real app, generate a link to frontend
        // var link = $"https://app.com/reset-password?email={email}&token={Uri.EscapeDataString(token)}";
        // await _emailService.SendEmailAsync(email, "Reset Password", $"Please reset your password by clicking here: {link}");
        
        // Since we don't know the FE URL, we send the raw token or a placeholder link
         await _emailService.SendEmailAsync(email, "Reset Password Token", $"Your Reset Token is: {token}");
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
