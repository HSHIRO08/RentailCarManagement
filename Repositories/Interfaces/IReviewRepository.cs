using RentailCarManagement.Models;
using RentailCarManagement.DTOs.Review;

namespace RentailCarManagement.Repositories.Interfaces;

/// <summary>
/// Review Repository Interface
/// </summary>
public interface IReviewRepository : IGenericRepository<Review>
{
    /// <summary>
    /// Lấy đánh giá của xe
    /// </summary>
    Task<IEnumerable<Review>> GetReviewsByCarAsync(Guid carId);

    /// <summary>
    /// Lấy đánh giá của khách hàng
    /// </summary>
    Task<IEnumerable<Review>> GetReviewsByCustomerAsync(Guid customerId);

    /// <summary>
    /// Lấy đánh giá của đơn thuê
    /// </summary>
    Task<Review?> GetReviewByRentalAsync(Guid rentalId);

    /// <summary>
    /// Tìm kiếm đánh giá
    /// </summary>
    Task<(IEnumerable<Review> Reviews, int TotalCount)> SearchReviewsAsync(ReviewFilterCriteria criteria);

    /// <summary>
    /// Tính rating trung bình của xe
    /// </summary>
    Task<decimal?> GetAverageRatingByCarAsync(Guid carId);

    /// <summary>
    /// Tính rating trung bình của nhà cung cấp
    /// </summary>
    Task<decimal?> GetAverageRatingBySupplierAsync(Guid supplierId);

    /// <summary>
    /// Kiểm tra khách hàng đã đánh giá đơn thuê chưa
    /// </summary>
    Task<bool> HasCustomerReviewedRentalAsync(Guid customerId, Guid rentalId);
}
