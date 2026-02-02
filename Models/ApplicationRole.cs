using Microsoft.AspNetCore.Identity;

namespace RentailCarManagement.Models;

/// <summary>
/// Application Role - Kế thừa từ IdentityRole để sử dụng ASP.NET Core Identity
/// </summary>
public class ApplicationRole : IdentityRole<Guid>
{
    /// <summary>
    /// Mô tả role
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Ngày tạo
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    public ApplicationRole() : base()
    {
    }

    public ApplicationRole(string roleName) : base(roleName)
    {
    }

    public ApplicationRole(string roleName, string description) : base(roleName)
    {
        Description = description;
    }
}
