using System.ComponentModel.DataAnnotations;

namespace RentailCarManagement.DTOs.Rental;

/// <summary>
/// DTO để tạo đơn thuê xe mới
/// </summary>
public class CreateRentalRequest
{
    [Required(ErrorMessage = "ID xe là bắt buộc")]
    public Guid CarId { get; set; }

    [Required(ErrorMessage = "ID khách hàng là bắt buộc")]
    public Guid CustomerId { get; set; }

    [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc")]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "Ngày kết thúc là bắt buộc")]
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Địa điểm nhận xe
    /// </summary>
    [StringLength(500)]
    public string? PickupLocation { get; set; }

    /// <summary>
    /// Địa điểm trả xe
    /// </summary>
    [StringLength(500)]
    public string? ReturnLocation { get; set; }

    /// <summary>
    /// Thông tin người lái (nếu khác khách hàng)
    /// </summary>
    [StringLength(100)]
    public string? DriverName { get; set; }

    [StringLength(50)]
    public string? DriverLicense { get; set; }

    [StringLength(20)]
    public string? DriverPhone { get; set; }

    /// <summary>
    /// Yêu cầu đặc biệt
    /// </summary>
    [StringLength(1000)]
    public string? SpecialRequests { get; set; }

    /// <summary>
    /// Mã khuyến mãi
    /// </summary>
    [StringLength(50)]
    public string? CouponCode { get; set; }

    /// <summary>
    /// Ghi chú
    /// </summary>
    [StringLength(1000)]
    public string? Notes { get; set; }
}
