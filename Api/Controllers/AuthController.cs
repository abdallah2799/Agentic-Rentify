using MediatR;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="command">The registration details.</param>
    /// <returns>The created user's auth details.</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterCommand command)
    {
        return Ok(await _mediator.Send(command));
    }

    /// <summary>
    /// Logs in an existing user.
    /// </summary>
    /// <param name="command">Login credentials.</param>
    /// <returns>JWT Access Token and Refresh Token.</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginCommand command)
    {
        return Ok(await _mediator.Send(command));
    }

    /// <summary>
    /// Logs in using Google ID Token.
    /// </summary>
    /// <param name="command">Google ID Token.</param>
    /// <returns>JWT Access Token and Refresh Token.</returns>
    [HttpPost("google-login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponseDto>> GoogleLogin([FromBody] GoogleLoginCommand command)
    {
        return Ok(await _mediator.Send(command));
    }

    /// <summary>
    /// Refreshes the Access Token using a Refresh Token.
    /// </summary>
    /// <param name="command">The expired token and valid refresh token.</param>
    /// <returns>New JWT Access Token and Refresh Token.</returns>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponseDto>> RefreshToken([FromBody] RefreshTokenCommand command)
    {
        return Ok(await _mediator.Send(command));
    }

    /// <summary>
    /// Initiates the forgot password process.
    /// </summary>
    /// <param name="command">The user's email.</param>
    /// <returns>Success message.</returns>
    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
    {
        await _mediator.Send(command);
        return Ok(new { message = "If the email exists, a reset link/token has been sent." });
    }

    /// <summary>
    /// Resets the user's password using a token.
    /// </summary>
    /// <param name="command">The email, token, and new password.</param>
    /// <returns>Success message.</returns>
    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
    {
        await _mediator.Send(command);
        return Ok(new { message = "Password reset successfully." });
    }
    
    /// <summary>
    /// Changes the password for the current user.
    /// </summary>
    /// <param name="command">Email, current password, and new password.</param>
    /// <returns>Success message.</returns>
    [HttpPost("change-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
    {
        await _mediator.Send(command);
        return Ok(new { message = "Password changed successfully." });
    }
    
    /// <summary>
    /// Validates if a JWT token is valid.
    /// </summary>
    /// <param name="token">The JWT token to validate.</param>
    /// <returns>True if valid, false otherwise.</returns>
    [HttpGet("validate-token")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> ValidateToken([FromQuery] string token)
    {
        return Ok(await _mediator.Send(new ValidateTokenQuery { Token = token }));
    }

    /// <summary>
    /// Verifies the user's email using the OTP code.
    /// </summary>
    /// <param name="command">Email and OTP Code.</param>
    /// <returns>Success message.</returns>
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
    /// Resends the OTP for email verification.
    /// </summary>
    /// <param name="command">The user's email.</param>
    /// <returns>Success message.</returns>
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
