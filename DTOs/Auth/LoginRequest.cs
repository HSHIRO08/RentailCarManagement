using Microsoft.OpenApi.MicrosoftExtensions;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace RentailCarManagement.DTOs.Auth;

/// <summary>
/// DTO đăng nhập
/// </summary>
public class LoginRequest
{
    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]

    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
    public string Password { get; set; } = null!;

    /// <summary>
    /// Ghi nhớ đăng nhập
    /// </summary>
    public bool RememberMe { get; set; } = false;
}
