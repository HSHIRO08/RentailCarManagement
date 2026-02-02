using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentailCarManagement.Models;

/// <summary>
/// B?ng trung gian Car - Insurance (Many-to-Many)
/// </summary>
public class CarInsurance
{
    [Key]
    public Guid CarInsuranceId { get; set; }

    /// <summary>
    /// ID xe
    /// </summary>
    public Guid CarId { get; set; }

    /// <summary>
    /// ID b?o hi?m
    /// </summary>
    public Guid InsuranceId { get; set; }

    /// <summary>
    /// Ngày b?t ??u
    /// </summary>
    [Required]
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Ngày k?t thúc
    /// </summary>
    [Required]
    public DateTime EndDate { get; set; }

    /// <summary>
    /// S? h?p ??ng b?o hi?m
    /// </summary>
    [StringLength(100)]
    public string? PolicyNumber { get; set; }

    /// <summary>
    /// Tr?ng thái ho?t ??ng
    /// </summary>
    public bool IsActive { get; set; } = true;

    public DateTime? CreatedAt { get; set; }

    // Navigation Properties
    [ForeignKey("CarId")]
    public virtual Car Car { get; set; } = null!;

    [ForeignKey("InsuranceId")]
    public virtual Insurance Insurance { get; set; } = null!;
}
