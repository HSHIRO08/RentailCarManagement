namespace RentailCarManagement.DTOs.Rental;

/// <summary>
/// DTO trả về thông tin đơn thuê (danh sách)
/// </summary>
public class RentalResponse
{
    public Guid RentalId { get; set; }
    public Guid CarId { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Status { get; set; }
    public DateTime? CreatedAt { get; set; }

    // Related data
    public string? CarBrand { get; set; }
    public string? CarModel { get; set; }
    public string? CarLicensePlate { get; set; }
    public string? CarImageUrl { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public int TotalDays { get; set; }
    public string? PaymentStatus { get; set; }
}
