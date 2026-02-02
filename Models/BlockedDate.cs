using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RentailCarManagement.Enums;

namespace RentailCarManagement.Models;

/// <summary>
/// Ngày b? ch?n (không cho thuê)
/// </summary>
public class BlockedDate
{
    [Key]
    public Guid BlockedDateId { get; set; }

    /// <summary>
    /// ID xe
    /// </summary>
    public Guid CarId { get; set; }

    /// <summary>
    /// Ngày b?t ??u ch?n
    /// </summary>
    [Required]
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Ngày k?t thúc ch?n
    /// </summary>
    [Required]
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Lý do ch?n
    /// </summary>
    [Required]
    public BlockedReason Reason { get; set; }

    /// <summary>
    /// Ghi chú chi ti?t
    /// </summary>
    [StringLength(500)]
    public string? Notes { get; set; }

    /// <summary>
    /// Ng??i t?o
    /// </summary>
    public Guid? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    // Navigation Properties
    [ForeignKey("CarId")]
    public virtual Car Car { get; set; } = null!;

    [ForeignKey("CreatedBy")]
    public virtual AspNetUser? Creator { get; set; }
}
