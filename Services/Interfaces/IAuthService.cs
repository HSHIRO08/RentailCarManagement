using RentailCarManagement.DTOs.Auth;

namespace RentailCarManagement.Services.Interfaces;

/// <summary>
/// Authentication Service Interface
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Đăng ký tài khoản mới
    /// </summary>
    Task<AuthResponse> RegisterAsync(RegisterRequest request);

    /// <summary>
    /// Đăng nhập
    /// </summary>
    Task<AuthResponse> LoginAsync(LoginRequest request);

    /// <summary>
    /// Làm mới token
    /// </summary>
    Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request);

    /// <summary>
    /// Yêu cầu reset mật khẩu
    /// </summary>
    Task<bool> ForgotPasswordAsync(ForgotPasswordRequest request);

    /// <summary>
    /// Reset mật khẩu
    /// </summary>
    Task<bool> ResetPasswordAsync(ResetPasswordRequest request);

    /// <summary>
    /// Đăng xuất
    /// </summary>
    Task<bool> LogoutAsync(Guid userId);

    /// <summary>
    /// Xác thực email
    /// </summary>
    Task<bool> ConfirmEmailAsync(string email, string token);
}
