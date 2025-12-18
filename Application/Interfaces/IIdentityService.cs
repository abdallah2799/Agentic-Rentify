
public interface IIdentityService
{
    Task<AuthResponseDto> RegisterAsync(string email, string password, string firstName, string lastName, string gender, string nationality, DateTime dateOfBirth, string? profileImage);
    Task<AuthResponseDto> LoginAsync(string email, string password);
    Task<bool> ConfirmEmailAsync(string email, string code);
    Task ResendOtpAsync(string email);
    Task<AuthResponseDto> LoginWithGoogleAsync(string idToken);
    Task<AuthResponseDto> RefreshTokenAsync(string token, string refreshToken);
    
    Task ForgotPasswordAsync(string email);
    Task ResetPasswordAsync(string email, string token, string newPassword);
    Task ChangePasswordAsync(string email, string currentPassword, string newPassword);
    
    Task<bool> ValidateTokenAsync(string token);
}
