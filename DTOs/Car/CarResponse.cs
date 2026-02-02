namespace RentailCarManagement.DTOs.Car;

/// <summary>
/// DTO trả về thông tin xe (danh sách)
/// </summary>
public class CarResponse
{
    public Guid CarId { get; set; }
    public string LicensePlate { get; set; } = null!;
    public string Brand { get; set; } = null!;
    public string Model { get; set; } = null!;
    public int? Year { get; set; }
    public int? Seats { get; set; }
    public string? FuelType { get; set; }
    public string? Transmission { get; set; }
    public decimal PricePerDay { get; set; }
    public decimal? PricePerHour { get; set; }
    public string? Status { get; set; }
    public bool? IsApproved { get; set; }
    public string? CategoryName { get; set; }
    public string? SupplierName { get; set; }
    public string? PrimaryImageUrl { get; set; }
    public decimal? AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public DateTime? CreatedAt { get; set; }
}
