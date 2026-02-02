namespace RentailCarManagement.DTOs.Review;

/// <summary>
/// DTO trả về thông tin đánh giá
/// </summary>
public class ReviewResponse
{
    public Guid ReviewId { get; set; }
    public Guid RentalId { get; set; }
    public Guid CustomerId { get; set; }
    public int? Rating { get; set; }
    public string? Comment { get; set; }
    public bool? IsApproved { get; set; }
    public DateTime? CreatedAt { get; set; }

    // Related data
    public string? CustomerName { get; set; }
    public string? CustomerAvatar { get; set; }
    public string? CarBrand { get; set; }
    public string? CarModel { get; set; }

    // Response from supplier
    public string? SupplierResponse { get; set; }
    public DateTime? ResponseDate { get; set; }
}
