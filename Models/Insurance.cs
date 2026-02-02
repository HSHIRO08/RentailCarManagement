using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RentailCarManagement.Enums;

namespace RentailCarManagement.Models;

/// <summary>
/// Gói b?o hi?m
/// </summary>
public class Insurance
{
    [Key]
    public Guid InsuranceId { get; set; }

    /// <summary>
    /// Tên gói b?o hi?m
    /// </summary>
    [Required(ErrorMessage = "Tên gói b?o hi?m là b?t bu?c")]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Lo?i b?o hi?m
    /// </summary>
    [Required]
    public InsuranceType InsuranceType { get; set; }

    /// <summary>
    /// Mô t? ph?m vi b?o hi?m
    /// </summary>
    [StringLength(2000)]
    public string? Coverage { get; set; }

    /// <summary>
    /// Giá b?o hi?m theo ngày
    /// </summary>
    [Required]
    [Range(0, double.MaxValue)]
    [Column(TypeName = "decimal(18,2)")]
    public decimal PricePerDay { get; set; }

    /// <summary>
    /// M?c kh?u tr?
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal? Deductible { get; set; }

    /// <summary>
    /// ?i?u kho?n và ?i?u ki?n
    /// </summary>
    public string? TermsAndConditions { get; set; }

    /// <summary>
    /// Tr?ng thái ho?t ??ng
    /// </summary>
    public bool IsActive { get; set; } = true;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    // Navigation Properties
    public virtual ICollection<CarInsurance> CarInsurances { get; set; } = new List<CarInsurance>();
}
