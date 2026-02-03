using Microsoft.AspNetCore.Mvc;
using RentailCarManagement.DTOs.Review;
using RentailCarManagement.DTOs.Common;
using RentailCarManagement.Services.Interfaces;
using RentailCarManagement.Exceptions;

namespace RentailCarManagement.Controllers;

/// <summary>
/// API Controller cho quản lý đánh giá
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;
    private readonly ILogger<ReviewsController> _logger;

    public ReviewsController(IReviewService reviewService, ILogger<ReviewsController> logger)
    {
        _reviewService = reviewService;
        _logger = logger;
    }

    /// <summary>
    /// Tạo đánh giá mới
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ReviewResponse>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateReview([FromQuery] Guid customerId, [FromBody] CreateReviewRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse.FailResult("Dữ liệu không hợp lệ"));

        try
        {
            var review = await _reviewService.CreateReviewAsync(customerId, request);
            return CreatedAtAction(nameof(GetReviewDetails), new { id = review.ReviewId },
                ApiResponse<ReviewResponse>.SuccessResult(review, "Tạo đánh giá thành công"));
        }
        catch (BusinessException ex)
        {
            return BadRequest(ApiResponse.FailResult(ex.Message));
        }
    }

    /// <summary>
    /// Lấy chi tiết đánh giá
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ReviewResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReviewDetails(Guid id)
    {
        var review = await _reviewService.GetReviewDetailsAsync(id);
        if (review == null)
            return NotFound(ApiResponse.FailResult("Đánh giá không tồn tại"));

        return Ok(ApiResponse<ReviewResponse>.SuccessResult(review));
    }

    /// <summary>
    /// Xác nhận đánh giá (Admin)
    /// </summary>
    [HttpPut("{id:guid}/verify")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> VerifyReview(Guid id, [FromQuery] bool approved = true)
    {
        var result = await _reviewService.VerifyReviewAsync(id, approved);
        if (!result)
            return NotFound(ApiResponse.FailResult("Đánh giá không tồn tại"));

        return Ok(ApiResponse.SuccessResult(approved ? "Đánh giá đã được phê duyệt" : "Đánh giá đã bị từ chối"));
    }

    /// <summary>
    /// Phản hồi đánh giá (Supplier)
    /// </summary>
    [HttpPost("{id:guid}/respond")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> RespondToReview(Guid id, [FromQuery] Guid supplierId, [FromBody] ReviewResponseRequest request)
    {
        var result = await _reviewService.RespondToReviewAsync(id, supplierId, request.Response);
        if (!result)
            return BadRequest(ApiResponse.FailResult("Không thể phản hồi đánh giá này"));

        return Ok(ApiResponse.SuccessResult("Phản hồi thành công"));
    }

    /// <summary>
    /// Lấy đánh giá của xe
    /// </summary>
    [HttpGet("car/{carId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ReviewResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCarReviews(Guid carId, [FromQuery] ReviewFilterCriteria criteria)
    {
        var reviews = await _reviewService.GetCarReviewsAsync(carId, criteria);
        return Ok(ApiResponse<PagedResult<ReviewResponse>>.SuccessResult(reviews));
    }

    /// <summary>
    /// Lấy đánh giá của nhà cung cấp
    /// </summary>
    [HttpGet("supplier/{supplierId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ReviewResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSupplierReviews(Guid supplierId, [FromQuery] ReviewFilterCriteria criteria)
    {
        var reviews = await _reviewService.GetSupplierReviewsAsync(supplierId, criteria);
        return Ok(ApiResponse<PagedResult<ReviewResponse>>.SuccessResult(reviews));
    }

    /// <summary>
    /// Lấy rating trung bình của xe
    /// </summary>
    [HttpGet("car/{carId:guid}/average-rating")]
    [ProducesResponseType(typeof(ApiResponse<decimal?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAverageRating(Guid carId)
    {
        var rating = await _reviewService.CalculateAverageRatingAsync(carId);
        return Ok(ApiResponse<decimal?>.SuccessResult(rating));
    }

    /// <summary>
    /// Xóa đánh giá (Admin)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteReview(Guid id)
    {
        var result = await _reviewService.DeleteReviewAsync(id);
        if (!result)
            return NotFound(ApiResponse.FailResult("Đánh giá không tồn tại"));

        return Ok(ApiResponse.SuccessResult("Xóa đánh giá thành công"));
    }
}
