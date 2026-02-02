using RentailCarManagement.DTOs.Review;
using RentailCarManagement.DTOs.Common;

namespace RentailCarManagement.Services.Interfaces;

/// <summary>
/// Review Service Interface
/// </summary>
public interface IReviewService
{
    /// <summary>
    /// Tạo đánh giá mới
    /// </summary>
    Task<ReviewResponse> CreateReviewAsync(Guid customerId, CreateReviewRequest request);

    /// <summary>
    /// Lấy chi tiết đánh giá
    /// </summary>
    Task<ReviewResponse?> GetReviewDetailsAsync(Guid reviewId);

    /// <summary>
    /// Xác nhận đánh giá (Admin)
    /// </summary>
    Task<bool> VerifyReviewAsync(Guid reviewId, bool approved);

    /// <summary>
    /// Phản hồi đánh giá (Supplier)
    /// </summary>
    Task<bool> RespondToReviewAsync(Guid reviewId, Guid supplierId, string response);

    /// <summary>
    /// Lấy đánh giá của xe
    /// </summary>
    Task<PagedResult<ReviewResponse>> GetCarReviewsAsync(Guid carId, ReviewFilterCriteria criteria);

    /// <summary>
    /// Lấy đánh giá của nhà cung cấp
    /// </summary>
    Task<PagedResult<ReviewResponse>> GetSupplierReviewsAsync(Guid supplierId, ReviewFilterCriteria criteria);

    /// <summary>
    /// Tính rating trung bình của xe
    /// </summary>
    Task<decimal?> CalculateAverageRatingAsync(Guid carId);

    /// <summary>
    /// Xóa đánh giá
    /// </summary>
    Task<bool> DeleteReviewAsync(Guid reviewId);
}
