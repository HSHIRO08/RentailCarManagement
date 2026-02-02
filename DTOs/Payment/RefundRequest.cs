using System.ComponentModel.DataAnnotations;

namespace RentailCarManagement.DTOs.Payment;

/// <summary>
/// DTO để xử lý hoàn tiền
/// </summary>
public class RefundRequest
{
    [Required(ErrorMessage = "Số tiền hoàn là bắt buộc")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Số tiền phải lớn hơn 0")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Lý do hoàn tiền là bắt buộc")]
    [StringLength(500)]
    public string Reason { get; set; } = null!;

    [StringLength(500)]
    public string? Notes { get; set; }
}
