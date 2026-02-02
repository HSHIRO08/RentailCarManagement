using System.ComponentModel.DataAnnotations;

namespace RentailCarManagement.DTOs.Rental;

/// <summary>
/// DTO để hủy đơn thuê
/// </summary>
public class CancelRentalRequest
{
    [Required(ErrorMessage = "Lý do hủy là bắt buộc")]
    [StringLength(1000)]
    public string CancellationReason { get; set; } = null!;
}

/// <summary>
/// DTO để gia hạn đơn thuê
/// </summary>
public class ExtendRentalRequest
{
    [Required(ErrorMessage = "Số ngày gia hạn là bắt buộc")]
    [Range(1, 365, ErrorMessage = "Số ngày gia hạn phải từ 1-365")]
    public int ExtendedDays { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }
}
