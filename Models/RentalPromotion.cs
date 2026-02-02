using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentailCarManagement.Models;

/// <summary>
/// B?ng trung gian Rental - Promotion (Many-to-Many)
/// </summary>
public class RentalPromotion
{
    [Key]
    public Guid RentalPromotionId { get; set; }

    /// <summary>
    /// ID ??n thuê
    /// </summary>
    public Guid RentalId { get; set; }

    /// <summary>
    /// ID khuy?n mãi
    /// </summary>
    public Guid PromotionId { get; set; }

    /// <summary>
    /// S? ti?n ?ã gi?m
    /// </summary>
    [Column(TypeName = "decimal(18,2)")]
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// Ngày áp d?ng
    /// </summary>
    public DateTime AppliedAt { get; set; }

    // Navigation Properties
    [ForeignKey("RentalId")]
    public virtual Rental Rental { get; set; } = null!;

    [ForeignKey("PromotionId")]
    public virtual Promotion Promotion { get; set; } = null!;
}
