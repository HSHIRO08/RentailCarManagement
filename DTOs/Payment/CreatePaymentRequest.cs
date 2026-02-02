using System.ComponentModel.DataAnnotations;

namespace RentailCarManagement.DTOs.Payment;

/// <summary>
/// DTO để tạo thanh toán mới
/// </summary>
public class CreatePaymentRequest
{
    [Required(ErrorMessage = "ID đơn thuê là bắt buộc")]
    public Guid RentalId { get; set; }

    [Required(ErrorMessage = "Số tiền là bắt buộc")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Số tiền phải lớn hơn 0")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Phương thức thanh toán là bắt buộc")]
    [StringLength(50)]
    public string PaymentMethod { get; set; } = null!;

    /// <summary>
    /// Loại thanh toán (Deposit, Rental, Extra, Refund)
    /// </summary>
    [StringLength(50)]
    public string? PaymentType { get; set; }

    /// <summary>
    /// Mã giao dịch từ cổng thanh toán
    /// </summary>
    [StringLength(100)]
    public string? TransactionId { get; set; }

    /// <summary>
    /// Tên cổng thanh toán
    /// </summary>
    [StringLength(100)]
    public string? PaymentGateway { get; set; }

    /// <summary>
    /// Ghi chú
    /// </summary>
    [StringLength(500)]
    public string? Notes { get; set; }
}
