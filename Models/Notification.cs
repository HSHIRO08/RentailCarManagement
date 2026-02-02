using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RentailCarManagement.Enums;

namespace RentailCarManagement.Models;

/// <summary>
/// Thông báo h? th?ng
/// </summary>
public class Notification
{
    [Key]
    public Guid NotificationId { get; set; }

    /// <summary>
    /// ID ng??i dùng nh?n thông báo
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Lo?i thông báo
    /// </summary>
    [Required]
    public NotificationType NotificationType { get; set; }

    /// <summary>
    /// Tiêu ??
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = null!;

    /// <summary>
    /// N?i dung
    /// </summary>
    [Required]
    public string Message { get; set; } = null!;

    /// <summary>
    /// ?ã ??c ch?a
    /// </summary>
    public bool IsRead { get; set; } = false;

    /// <summary>
    /// Ngày g?i
    /// </summary>
    public DateTime SentAt { get; set; }

    /// <summary>
    /// Ngày ??c
    /// </summary>
    public DateTime? ReadAt { get; set; }

    /// <summary>
    /// Lo?i entity liên quan
    /// </summary>
    [StringLength(50)]
    public string? RelatedEntityType { get; set; }

    /// <summary>
    /// ID entity liên quan
    /// </summary>
    public Guid? RelatedEntityId { get; set; }

    /// <summary>
    /// URL ?? navigate khi click
    /// </summary>
    [StringLength(500)]
    public string? ActionUrl { get; set; }

    public DateTime? CreatedAt { get; set; }

    // Navigation Properties
    [ForeignKey("UserId")]
    public virtual AspNetUser User { get; set; } = null!;
}
