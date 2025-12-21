using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Agentic_Rentify.Api.Controllers;

/// <summary>
/// User authentication and account management
/// </summary>
/// <remarks>
/// Provides complete authentication flow including registration, login (email/password and Google OAuth),
/// OTP verification, password management, and JWT token refresh.
/// 
/// **Security Features:**
/// - JWT access tokens with refresh token rotation
/// - Account lockout after 5 failed login attempts (15 min duration)
/// - Rate limiting on sensitive endpoints
/// - Email OTP verification for account activation
/// - Secure password reset flow
/// </remarks>
[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
[ApiExplorerSettings(GroupName = "Authentication")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Registers a new user account.
    /// </summary>
    /// <remarks>
    /// Creates a new user account, generates an OTP, and sends a verification email.
    /// 
    /// **Password Requirements:**
    /// - Minimum 8 characters
    /// - At least one uppercase letter
    /// - At least one lowercase letter
    /// - At least one number
    /// - At least one special character
    /// 
    /// After registration, the user must verify their email using the POST /api/Auth/verify-otp endpoint.
    /// </remarks>
    /// <param name="command">Registration details including email, password, and personal info.</param>
    /// <returns>AuthResponseDto containing JWT tokens and user info.</returns>
    /// <response code="200">User registered successfully. Check email for OTP.</response>
    /// <response code="400">Invalid input or user already exists.</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterCommand command)
    {
        return Ok(await _mediator.Send(command));
    }

    /// <summary>
    /// Authenticates an existing user.
    /// </summary>
    /// <remarks>
    /// Validates credentials and returns JWT access token and refresh token.
    /// 
    /// **Security:**
    /// - Rate limiting applies
    /// - Account lockouts occur after 5 failed attempts (15 min duration)
    /// - Tokens expire after configured duration (check appsettings.json)
    /// 
    /// Use the returned access token in the Authorization header for protected endpoints:
    /// `Authorization: Bearer {accessToken}`
    /// </remarks>
    /// <param name="command">Credentials (Email and Password).</param>
    /// <returns>AuthResponseDto with JWT access token and refresh token.</returns>
    /// <response code="200">Login successful.</response>
    /// <response code="401">Invalid credentials or account locked.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginCommand command)
    {
        return Ok(await _mediator.Send(command));
    }

    /// <summary>
    /// Authenticates or registers a user using Google OAuth.
    /// </summary>
    /// <remarks>
    /// Validates the Google ID Token and creates/returns a user session.
    /// If the user doesn't exist, a new account is automatically created.
    /// 
    /// **Frontend Integration:**
    /// 1. Use Google Sign-In SDK to obtain ID token
    /// 2. Send token to this endpoint
    /// 3. Store returned JWT tokens
    /// </remarks>
    /// <param name="command">Google ID Token obtained from Google Sign-In.</param>
    /// <returns>AuthResponseDto with JWT tokens.</returns>
    /// <response code="200">Authenticated successfully.</response>
    /// <response code="400">Invalid Google Token.</response>
    [HttpPost("google-login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponseDto>> GoogleLogin([FromBody] GoogleLoginCommand command)
    {
        return Ok(await _mediator.Send(command));
    }

    /// <summary>
    /// Refreshes the access token using a valid refresh token.
    /// </summary>
    /// <remarks>
    /// Uses a valid refresh token to obtain a new access token without requiring user credentials.
    /// 
    /// **Token Rotation:**
    /// - Old access token is invalidated
    /// - New access token is issued
    /// - Refresh token may be rotated (check response)
    /// 
    /// Call this endpoint when you receive 401 Unauthorized due to expired access token.
    /// </remarks>
    /// <param name="command">The current tokens.</param>
    /// <returns>New tokens.</returns>
    /// <response code="200">Tokens refreshed.</response>
    /// <response code="400">Invalid or expired tokens.</response>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponseDto>> RefreshToken([FromBody] RefreshTokenCommand command)
    {
        return Ok(await _mediator.Send(command));
    }

    /// <summary>
    /// Initiates password reset flow.
    /// </summary>
    /// <remarks>
    /// Sends an email with a unique deep link to reset the password.
    /// Always returns 200 OK to prevent email enumeration.
    /// </remarks>
    /// <param name="command">User's email.</param>
    /// <returns>Generic success message.</returns>
    /// <response code="200">Request processed.</response>
    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
    {
        await _mediator.Send(command);
        return Ok(new { message = "If the email exists, a reset link/token has been sent." });
    }

    /// <summary>
    /// Completes password reset.
    /// </summary>
    /// <remarks>
    /// Sets a new password using the token received in email.
    /// </remarks>
    /// <param name="command">Token, email, and new password.</param>
    /// <returns>Success message.</returns>
    /// <response code="200">Password reset successful.</response>
    /// <response code="400">Invalid token or password complexity failure.</response>
    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
    {
        await _mediator.Send(command);
        return Ok(new { message = "Password reset successfully." });
    }
    
    /// <summary>
    /// Changes the password for the currently authenticated user.
    /// </summary>
    /// <remarks>
    /// Requires a valid Bearer token.
    /// </remarks>
    /// <param name="command">Current and new password.</param>
    /// <returns>Success message.</returns>
    /// <response code="200">Password changed.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="400">Incorrect current password.</response>
    [Authorize]
    [HttpPost("change-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
    {
        await _mediator.Send(command);
        return Ok(new { message = "Password changed successfully." });
    }
    
    /// <summary>
    /// Validates a JWT token's signature and expiration.
    /// </summary>
    /// <param name="token">The JWT string.</param>
    /// <returns>Boolean indicating validity.</returns>
    [HttpGet("validate-token")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> ValidateToken([FromQuery] string token)
    {
        return Ok(await _mediator.Send(new ValidateTokenQuery { Token = token }));
    }

    /// <summary>
    /// Verifies email using OTP.
    /// </summary>
    /// <remarks>
    /// Marks the user as verified if OTP is correct and not expired (10 min).
    /// </remarks>
    /// <param name="command">Email and OTP.</param>
    /// <returns>Success message.</returns>
    /// <response code="200">Verified.</response>
    /// <response code="400">Invalid or expired OTP.</response>
    [HttpPost("verify-email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailCommand command)
    {
        await _mediator.Send(command);
        return Ok(new { Message = "Email verified successfully" });
    }

    /// <summary>
    /// Resends the Email Verification OTP.
    /// </summary>
    /// <remarks>
    /// Requires a valid Bearer token (even if user is not verified).
    /// Generates a new 6-digit OTP and invalidates the old one.
    /// </remarks>
    /// <param name="command">User email.</param>
    /// <returns>Success message.</returns>
    /// <response code="200">OTP Sent.</response>
    /// <response code="401">Unauthorized.</response>
    [Authorize]
    [HttpPost("resend-otp")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResendOtp([FromBody] ResendOtpCommand command)
    {
        await _mediator.Send(command);
        return Ok(new { Message = "OTP resent successfully" });
    }
}
