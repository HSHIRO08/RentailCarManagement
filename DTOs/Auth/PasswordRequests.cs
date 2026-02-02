using System.ComponentModel.DataAnnotations;

namespace RentailCarManagement.DTOs.Auth;

/// <summary>
/// DTO yêu cầu quên mật khẩu
/// </summary>
public class ForgotPasswordRequest
{
    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; set; } = null!;
}

/// <summary>
/// DTO đặt lại mật khẩu
/// </summary>
public class ResetPasswordRequest
{
    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Token là bắt buộc")]
    public string Token { get; set; } = null!;

    [Required(ErrorMessage = "Mật khẩu mới là bắt buộc")]
    [StringLength(100, MinimumLength = 6)]
    public string NewPassword { get; set; } = null!;

    [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc")]
    [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
    public string ConfirmPassword { get; set; } = null!;
}

/// <summary>
/// DTO làm mới token
/// </summary>
public class RefreshTokenRequest
{
    [Required(ErrorMessage = "Access token là bắt buộc")]
    public string AccessToken { get; set; } = null!;

    [Required(ErrorMessage = "Refresh token là bắt buộc")]
    public string RefreshToken { get; set; } = null!;
}
