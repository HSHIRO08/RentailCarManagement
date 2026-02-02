using Microsoft.AspNetCore.Identity;

namespace RentailCarManagement.Models;

/// <summary>
/// Application User - Kế thừa từ IdentityUser để sử dụng ASP.NET Core Identity
/// </summary>
public class ApplicationUser : IdentityUser<Guid>
{
    /// <summary>
    /// Họ tên đầy đủ
    /// </summary>
    public string FullName { get; set; } = null!;

    /// <summary>
    /// Trạng thái tài khoản
    /// </summary>
    public string Status { get; set; } = "Active";

    /// <summary>
    /// Ngày tạo
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Ngày cập nhật
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    // Navigation Properties
    public virtual ICollection<ChatSession> ChatSessions { get; set; } = new List<ChatSession>();
    public virtual ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
    public virtual Customer? Customer { get; set; }
    public virtual Supplier? Supplier { get; set; }
}
