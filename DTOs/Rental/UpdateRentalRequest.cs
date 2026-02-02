using System.ComponentModel.DataAnnotations;

namespace RentailCarManagement.DTOs.Rental;

/// <summary>
/// DTO để cập nhật đơn thuê xe
/// </summary>
public class UpdateRentalRequest
{
    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [StringLength(500)]
    public string? PickupLocation { get; set; }

    [StringLength(500)]
    public string? ReturnLocation { get; set; }

    [StringLength(100)]
    public string? DriverName { get; set; }

    [StringLength(50)]
    public string? DriverLicense { get; set; }

    [StringLength(20)]
    public string? DriverPhone { get; set; }

    [StringLength(1000)]
    public string? SpecialRequests { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }
}
