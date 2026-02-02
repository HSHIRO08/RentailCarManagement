using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RentailCarManagement.Enums;

namespace RentailCarManagement.Models;

/// <summary>
/// Yêu c?u gia h?n thuê xe
/// </summary>
public class RentalExtension
{
    [Key]
    public Guid ExtensionId { get; set; }

    /// <summary>
    /// ID ??n thuê g?c
    /// </summary>
    public Guid RentalId { get; set; }

    /// <summary>
    /// S? ngày gia h?n
    /// </summary>
    [Required]
    [Range(1, 365, ErrorMessage = "S? ngày gia h?n ph?i t? 1-365")]
    public int ExtendedDays { get; set; }

    /// <summary>
    /// Ngày k?t thúc c?
    /// </summary>
    [Required]
    public DateTime OldEndDate { get; set; }

    /// <summary>
    /// Ngày k?t thúc m?i
    /// </summary>
    [Required]
    public DateTime NewEndDate { get; set; }

    /// <summary>
    /// S? ti?n ph? thu
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal AdditionalAmount { get; set; }

    /// <summary>
    /// Ngày yêu c?u
    /// </summary>
    public DateTime RequestDate { get; set; }

    /// <summary>
    /// Tr?ng thái phê duy?t
    /// </summary>
    public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Pending;

    /// <summary>
    /// Ngày phê duy?t/t? ch?i
    /// </summary>
    public DateTime? ProcessedAt { get; set; }

    /// <summary>
    /// Ng??i x? lý
    /// </summary>
    public Guid? ProcessedBy { get; set; }

    /// <summary>
    /// Lý do (n?u t? ch?i)
    /// </summary>
    [StringLength(500)]
    public string? Reason { get; set; }

    /// <summary>
    /// Ghi chú
    /// </summary>
    public string? Notes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    // Navigation Properties
    [ForeignKey("RentalId")]
    public virtual Rental Rental { get; set; } = null!;

    [ForeignKey("ProcessedBy")]
    public virtual AspNetUser? Processor { get; set; }
}
