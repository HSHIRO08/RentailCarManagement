using System.ComponentModel.DataAnnotations;

namespace RentailCarManagement.DTOs.Car;

/// <summary>
/// DTO để cập nhật xe
/// </summary>
public class UpdateCarRequest
{
    public Guid? CategoryId { get; set; }

    [StringLength(20, MinimumLength = 5)]
    public string? LicensePlate { get; set; }

    [StringLength(50)]
    public string? Brand { get; set; }

    [StringLength(50)]
    public string? Model { get; set; }

    [Range(1900, 2100)]
    public int? Year { get; set; }

    [Range(2, 50)]
    public int? Seats { get; set; }

    [StringLength(50)]
    public string? FuelType { get; set; }

    [StringLength(50)]
    public string? Transmission { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? PricePerDay { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? PricePerHour { get; set; }

    [StringLength(50)]
    public string? Status { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(500)]
    public string? Features { get; set; }

    [StringLength(200)]
    public string? CurrentLocation { get; set; }
}
