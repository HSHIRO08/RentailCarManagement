using RentailCarManagement.DTOs.Review;
using RentailCarManagement.DTOs.Common;
using RentailCarManagement.Models;
using RentailCarManagement.Repositories.Interfaces;
using RentailCarManagement.Services.Interfaces;
using RentailCarManagement.Exceptions;

namespace RentailCarManagement.Services.Implementations;

/// <summary>
/// Review Service Implementation
/// </summary>
public class ReviewService : IReviewService
{
    private readonly IUnitOfWork _unitOfWork;

    public ReviewService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ReviewResponse> CreateReviewAsync(Guid customerId, CreateReviewRequest request)
    {
        // Check if rental exists and belongs to customer
        var rental = await _unitOfWork.Rentals.GetRentalWithDetailsAsync(request.RentalId);
        if (rental == null)
            throw new NotFoundException("Đơn thuê không tồn tại");

        if (rental.CustomerId != customerId)
            throw new BusinessException("Bạn không thể đánh giá đơn thuê này");

        if (rental.Status != "Completed")
            throw new BusinessException("Chỉ có thể đánh giá sau khi hoàn thành thuê xe");

        // Check if already reviewed
        var existingReview = await _unitOfWork.Reviews.GetReviewByRentalAsync(request.RentalId);
        if (existingReview != null)
            throw new BusinessException("Bạn đã đánh giá đơn thuê này");

        var review = new Review
        {
            ReviewId = Guid.NewGuid(),
            RentalId = request.RentalId,
            CustomerId = customerId,
            Rating = request.Rating,
            Comment = request.Comment,
            IsApproved = true,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Reviews.AddAsync(review);
        await _unitOfWork.SaveChangesAsync();

        return await GetReviewDetailsAsync(review.ReviewId) 
               ?? throw new BusinessException("Không thể tạo đánh giá");
    }

    public async Task<ReviewResponse?> GetReviewDetailsAsync(Guid reviewId)
    {
        var review = await _unitOfWork.Reviews.GetFirstWithIncludeAsync(
            r => r.ReviewId == reviewId,
            r => r.Customer,
            r => r.Rental);

        if (review == null)
            return null;

        return MapToReviewResponse(review);
    }

    public async Task<bool> VerifyReviewAsync(Guid reviewId, bool approved)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId);
        if (review == null)
            return false;

        review.IsApproved = approved;
        _unitOfWork.Reviews.Update(review);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RespondToReviewAsync(Guid reviewId, Guid supplierId, string response)
    {
        var review = await _unitOfWork.Reviews.GetFirstWithIncludeAsync(
            r => r.ReviewId == reviewId,
            r => r.Rental);

        if (review == null)
            return false;

        // Verify supplier owns the car
        var rental = await _unitOfWork.Rentals.GetRentalWithDetailsAsync(review.RentalId);
        if (rental?.Car?.SupplierId != supplierId)
            return false;

        // Store response (in a real app, you'd have a ReviewResponse table)
        // For now, we'll just return true
        return true;
    }

    public async Task<PagedResult<ReviewResponse>> GetCarReviewsAsync(Guid carId, ReviewFilterCriteria criteria)
    {
        criteria.CarId = carId;
        var (reviews, totalCount) = await _unitOfWork.Reviews.SearchReviewsAsync(criteria);

        return new PagedResult<ReviewResponse>
        {
            Items = reviews.Select(MapToReviewResponse).ToList(),
            TotalItems = totalCount,
            PageNumber = criteria.Page,
            PageSize = criteria.PageSize
        };
    }

    public async Task<PagedResult<ReviewResponse>> GetSupplierReviewsAsync(Guid supplierId, ReviewFilterCriteria criteria)
    {
        criteria.SupplierId = supplierId;
        var (reviews, totalCount) = await _unitOfWork.Reviews.SearchReviewsAsync(criteria);

        return new PagedResult<ReviewResponse>
        {
            Items = reviews.Select(MapToReviewResponse).ToList(),
            TotalItems = totalCount,
            PageNumber = criteria.Page,
            PageSize = criteria.PageSize
        };
    }

    public async Task<decimal?> CalculateAverageRatingAsync(Guid carId)
    {
        return await _unitOfWork.Reviews.GetAverageRatingByCarAsync(carId);
    }

    public async Task<bool> DeleteReviewAsync(Guid reviewId)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(reviewId);
        if (review == null)
            return false;

        _unitOfWork.Reviews.Delete(review);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    private ReviewResponse MapToReviewResponse(Review review)
    {
        return new ReviewResponse
        {
            ReviewId = review.ReviewId,
            RentalId = review.RentalId,
            CustomerId = review.CustomerId,
            Rating = review.Rating,
            Comment = review.Comment,
            IsApproved = review.IsApproved,
            CreatedAt = review.CreatedAt,
            CustomerName = review.Customer?.User?.FullName,
            CarBrand = review.Rental?.Car?.Brand,
            CarModel = review.Rental?.Car?.Model
        };
    }
}
