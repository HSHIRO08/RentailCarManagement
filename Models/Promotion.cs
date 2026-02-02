using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RentailCarManagement.Enums;

namespace RentailCarManagement.Models;

/// <summary>
/// Mã khuy?n mãi/Coupon
/// </summary>
public class Promotion
{
    [Key]
    public Guid PromotionId { get; set; }

    /// <summary>
    /// Mã coupon
    /// </summary>
    [Required(ErrorMessage = "Mã coupon là b?t bu?c")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Mã coupon ph?i t? 3-50 ký t?")]
    public string CouponCode { get; set; } = null!;

    /// <summary>
    /// Tên khuy?n mãi
    /// </summary>
    [Required(ErrorMessage = "Tên khuy?n mãi là b?t bu?c")]
    [StringLength(200)]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Mô t?
    /// </summary>
    [StringLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Lo?i gi?m giá (Percentage ho?c FixedAmount)
    /// </summary>
    [Required]
    public DiscountType DiscountType { get; set; }

    /// <summary>
    /// Giá tr? gi?m (ph?n tr?m ho?c s? ti?n)
    /// </summary>
    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Giá tr? gi?m ph?i l?n h?n 0")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal DiscountValue { get; set; }

    /// <summary>
    /// Gi?m t?i ?a (áp d?ng khi gi?m theo %)
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal? MaxDiscount { get; set; }

    /// <summary>
    /// S? ngày thuê t?i thi?u ?? áp d?ng
    /// </summary>
    [Range(1, 365)]
    public int? MinRentalDays { get; set; }

    /// <summary>
    /// S? ti?n ??n hàng t?i thi?u
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal? MinOrderAmount { get; set; }

    /// <summary>
    /// Ngày b?t ??u hi?u l?c
    /// </summary>
    [Required]
    public DateTime ValidFrom { get; set; }

    /// <summary>
    /// Ngày k?t thúc hi?u l?c
    /// </summary>
    [Required]
    public DateTime ValidTo { get; set; }

    /// <summary>
    /// Gi?i h?n s? l?n s? d?ng
    /// </summary>
    public int? UsageLimit { get; set; }

    /// <summary>
    /// S? l?n ?ã s? d?ng
    /// </summary>
    public int UsedCount { get; set; } = 0;

    /// <summary>
    /// Danh sách danh m?c xe áp d?ng (JSON array of CategoryIds)
    /// </summary>
    public string? ApplicableCarCategories { get; set; }

    /// <summary>
    /// Tr?ng thái ho?t ??ng
    /// </summary>
    public bool IsActive { get; set; } = true;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    // Navigation Properties
    public virtual ICollection<RentalPromotion> RentalPromotions { get; set; } = new List<RentalPromotion>();
}
