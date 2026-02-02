namespace RentailCarManagement.DTOs.Auth;

/// <summary>
/// DTO trả về sau đăng nhập thành công
/// </summary>
public class AuthResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public UserInfo? User { get; set; }
}

public class UserInfo
{
    public Guid UserId { get; set; }
    public string? Email { get; set; }
    public string? FullName { get; set; }
    public string? Phone { get; set; }
    public List<string> Roles { get; set; } = new();
    public string? Avatar { get; set; }
}
