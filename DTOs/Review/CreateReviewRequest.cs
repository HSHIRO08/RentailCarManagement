using System.ComponentModel.DataAnnotations;

namespace RentailCarManagement.DTOs.Review;

/// <summary>
/// DTO để tạo đánh giá
/// </summary>
public class CreateReviewRequest
{
    [Required(ErrorMessage = "ID đơn thuê là bắt buộc")]
    public Guid RentalId { get; set; }

    [Required(ErrorMessage = "Đánh giá là bắt buộc")]
    [Range(1, 5, ErrorMessage = "Đánh giá phải từ 1-5 sao")]    
    public int Rating { get; set; }

    [StringLength(2000)]
    public string? Comment { get; set; }
}
