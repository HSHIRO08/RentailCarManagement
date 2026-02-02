using System.ComponentModel.DataAnnotations;

namespace RentailCarManagement.DTOs.Car;

/// <summary>
/// DTO để tạo xe mới
/// </summary>
public class CreateCarRequest
{
    [Required(ErrorMessage = "Nhà cung cấp là bắt buộc")]
    public Guid SupplierId { get; set; }

    [Required(ErrorMessage = "Danh mục là bắt buộc")]
    public Guid CategoryId { get; set; }

    [Required(ErrorMessage = "Biển số xe là bắt buộc")]
    [StringLength(20, MinimumLength = 5, ErrorMessage = "Biển số xe phải từ 5-20 ký tự")]
    public string LicensePlate { get; set; } = null!;

    [Required(ErrorMessage = "Hãng xe là bắt buộc")]
    [StringLength(50)]
    public string Brand { get; set; } = null!;

    [Required(ErrorMessage = "Model xe là bắt buộc")]
    [StringLength(50)]
    public string Model { get; set; } = null!;

    [Range(1900, 2100, ErrorMessage = "Năm sản xuất không hợp lệ")]
    public int? Year { get; set; }

    [Range(2, 50, ErrorMessage = "Số ghế phải từ 2-50")]
    public int? Seats { get; set; }

    [StringLength(50)]
    public string? FuelType { get; set; }

    [StringLength(50)]
    public string? Transmission { get; set; }

    [Required(ErrorMessage = "Giá thuê theo ngày là bắt buộc")]
    [Range(0, double.MaxValue, ErrorMessage = "Giá thuê phải lớn hơn 0")]
    public decimal PricePerDay { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? PricePerHour { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(500)]
    public string? Features { get; set; }

    [StringLength(200)]
    public string? CurrentLocation { get; set; }
}
