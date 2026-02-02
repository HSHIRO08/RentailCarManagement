using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RentailCarManagement.Enums;

namespace RentailCarManagement.Models;

/// <summary>
/// L?ch s? b?o trì xe
/// </summary>
public class MaintenanceRecord
{
    [Key]
    public Guid MaintenanceId { get; set; }

    /// <summary>
    /// ID xe
    /// </summary>
    public Guid CarId { get; set; }

    /// <summary>
    /// Lo?i b?o trì
    /// </summary>
    [Required]
    public MaintenanceType MaintenanceType { get; set; }

    /// <summary>
    /// Tiêu ??
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = null!;

    /// <summary>
    /// Mô t? chi ti?t
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Ngày b?o trì
    /// </summary>
    [Required]
    public DateTime MaintenanceDate { get; set; }

    /// <summary>
    /// Chi phí
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal? Cost { get; set; }

    /// <summary>
    /// S? km hi?n t?i
    /// </summary>
    public int? CurrentMileage { get; set; }

    /// <summary>
    /// Ngày b?o trì ti?p theo
    /// </summary>
    public DateTime? NextMaintenanceDate { get; set; }

    /// <summary>
    /// S? km b?o trì ti?p theo
    /// </summary>
    public int? NextMaintenanceMileage { get; set; }

    /// <summary>
    /// Ng??i/??n v? th?c hi?n
    /// </summary>
    [StringLength(200)]
    public string? PerformedBy { get; set; }

    /// <summary>
    /// Ghi chú
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// ???ng d?n hóa ??n/biên lai
    /// </summary>
    public string? ReceiptUrl { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    // Navigation Properties
    [ForeignKey("CarId")]
    public virtual Car Car { get; set; } = null!;
}
