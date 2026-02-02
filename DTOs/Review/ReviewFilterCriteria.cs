using System.ComponentModel.DataAnnotations;

namespace RentailCarManagement.DTOs.Review;

/// <summary>
/// DTO để phản hồi đánh giá (từ Supplier)
/// </summary>
public class ReviewResponseRequest
{
    [Required(ErrorMessage = "Nội dung phản hồi là bắt buộc")]
    [StringLength(1000)]
    public string Response { get; set; } = null!;
}

/// <summary>
/// Tiêu chí lọc đánh giá
/// </summary>
public class ReviewFilterCriteria
{
    public Guid? CarId { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? SupplierId { get; set; }
    public int? MinRating { get; set; }
    public int? MaxRating { get; set; }
    public bool? IsApproved { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? SortBy { get; set; }
    public string? SortOrder { get; set; } = "desc";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
