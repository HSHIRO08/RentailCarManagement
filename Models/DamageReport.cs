using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RentailCarManagement.Enums;

namespace RentailCarManagement.Models;

/// <summary>
/// Báo cáo h? h?ng
/// </summary>
public class DamageReport
{
    [Key]
    public Guid DamageReportId { get; set; }

    /// <summary>
    /// ID ??n thuê liên quan
    /// </summary>
    public Guid RentalId { get; set; }

    /// <summary>
    /// ID xe
    /// </summary>
    public Guid CarId { get; set; }

    /// <summary>
    /// Ng??i báo cáo
    /// </summary>
    public Guid ReportedBy { get; set; }

    /// <summary>
    /// Ngày báo cáo
    /// </summary>
    public DateTime ReportDate { get; set; }

    /// <summary>
    /// Lo?i h? h?ng
    /// </summary>
    [Required]
    [StringLength(100)]
    public string DamageType { get; set; } = null!;

    /// <summary>
    /// Mô t? chi ti?t
    /// </summary>
    [Required]
    public string Description { get; set; } = null!;

    /// <summary>
    /// M?c ?? nghiêm tr?ng
    /// </summary>
    [Required]
    public DamageSeverity Severity { get; set; }

    /// <summary>
    /// Chi phí ??c tính
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal? EstimatedCost { get; set; }

    /// <summary>
    /// Chi phí th?c t?
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal? ActualCost { get; set; }

    /// <summary>
    /// Tr?ng thái x? lý
    /// </summary>
    [StringLength(50)]
    public string Status { get; set; } = "Pending";

    /// <summary>
    /// Ngày gi?i quy?t
    /// </summary>
    public DateTime? ResolvedAt { get; set; }

    /// <summary>
    /// Ghi chú gi?i quy?t
    /// </summary>
    public string? Resolution { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    // Navigation Properties
    [ForeignKey("RentalId")]
    public virtual Rental Rental { get; set; } = null!;

    [ForeignKey("CarId")]
    public virtual Car Car { get; set; } = null!;

    [ForeignKey("ReportedBy")]
    public virtual AspNetUser Reporter { get; set; } = null!;

    public virtual ICollection<DamagePhoto> DamagePhotos { get; set; } = new List<DamagePhoto>();
}
