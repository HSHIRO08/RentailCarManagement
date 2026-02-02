namespace RentailCarManagement.DTOs.Rental;

/// <summary>
/// DTO trả về thông tin chi tiết đơn thuê
/// </summary>
public class RentalDetailResponse
{
    public Guid RentalId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime? ActualReturnDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal? DepositAmount { get; set; }
    public string? Status { get; set; }
    public string? PickupLocation { get; set; }
    public string? ReturnLocation { get; set; }
    public string? DriverName { get; set; }
    public string? DriverLicense { get; set; }
    public string? DriverPhone { get; set; }
    public string? SpecialRequests { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime? CancellationDate { get; set; }
    public string? Notes { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Related data
    public RentalCarDto? Car { get; set; }
    public RentalCustomerDto? Customer { get; set; }
    public RentalPaymentDto? Payment { get; set; }
    public RentalReviewDto? Review { get; set; }
    public List<RentalPromotionDto> AppliedPromotions { get; set; } = new();
    public int TotalDays { get; set; }
}

public class RentalCarDto
{
    public Guid CarId { get; set; }
    public string Brand { get; set; } = null!;
    public string Model { get; set; } = null!;
    public string LicensePlate { get; set; } = null!;
    public int? Year { get; set; }
    public string? FuelType { get; set; }
    public string? Transmission { get; set; }
    public decimal PricePerDay { get; set; }
    public string? ImageUrl { get; set; }
}

public class RentalCustomerDto
{
    public Guid CustomerId { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? DriverLicenseNumber { get; set; }
}

public class RentalPaymentDto
{
    public Guid PaymentId { get; set; }
    public decimal Amount { get; set; }
    public string? PaymentMethod { get; set; }
    public string? Status { get; set; }
    public DateTime? PaidAt { get; set; }
}

public class RentalReviewDto
{
    public Guid ReviewId { get; set; }
    public int? Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime? CreatedAt { get; set; }
}

public class RentalPromotionDto
{
    public string CouponCode { get; set; } = null!;
    public string PromotionName { get; set; } = null!;
    public decimal DiscountAmount { get; set; }
}
