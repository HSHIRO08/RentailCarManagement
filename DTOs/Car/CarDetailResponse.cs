namespace RentailCarManagement.DTOs.Car;

/// <summary>
/// DTO trả về thông tin chi tiết xe
/// </summary>
public class CarDetailResponse
{
    public Guid CarId { get; set; }
    public Guid SupplierId { get; set; }
    public Guid CategoryId { get; set; }
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
    public string? Description { get; set; }
    public string? Features { get; set; }
    public string? CurrentLocation { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Related data
    public CarCategoryDto? Category { get; set; }
    public SupplierDto? Supplier { get; set; }
    public List<CarImageDto> Images { get; set; } = new();
    public List<CarDocumentDto> Documents { get; set; } = new();
    public decimal? AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public int TotalRentals { get; set; }
}

public class CarCategoryDto
{
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}

public class SupplierDto
{
    public Guid SupplierId { get; set; }
    public string? CompanyName { get; set; }
    public bool? IsVerified { get; set; }
}

public class CarImageDto
{
    public Guid ImageId { get; set; }
    public string ImageUrl { get; set; } = null!;
    public bool? IsPrimary { get; set; }
    public int? DisplayOrder { get; set; }
}

public class CarDocumentDto
{
    public Guid DocumentId { get; set; }
    public string DocumentType { get; set; } = null!;
    public string? DocumentNumber { get; set; }
    public DateOnly? ExpiryDate { get; set; }
    public bool? IsVerified { get; set; }
}
