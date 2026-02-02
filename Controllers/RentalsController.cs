using Microsoft.AspNetCore.Mvc;
using RentailCarManagement.DTOs.Rental;
using RentailCarManagement.DTOs.Common;
using RentailCarManagement.Services.Interfaces;
using RentailCarManagement.Exceptions;

namespace RentailCarManagement.Controllers;

/// <summary>
/// API Controller cho quản lý đơn thuê xe
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class RentalsController : ControllerBase
{
    private readonly IRentalService _rentalService;
    private readonly ILogger<RentalsController> _logger;

    public RentalsController(IRentalService rentalService, ILogger<RentalsController> logger)
    {
        _rentalService = rentalService;
        _logger = logger;
    }

    /// <summary>
    /// Tạo đơn thuê xe mới
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<RentalDetailResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateRental([FromBody] CreateRentalRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse.FailResult("Dữ liệu không hợp lệ"));

        try
        {
            var rental = await _rentalService.CreateRentalAsync(request);
            return CreatedAtAction(nameof(GetRentalDetails), new { id = rental.RentalId },
                ApiResponse<RentalDetailResponse>.SuccessResult(rental, "Tạo đơn thuê thành công"));
        }
        catch (BusinessException ex)
        {
            return BadRequest(ApiResponse.FailResult(ex.Message));
        }
    }

    /// <summary>
    /// Lấy chi tiết đơn thuê
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<RentalDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRentalDetails(Guid id)
    {
        var rental = await _rentalService.GetRentalDetailsAsync(id);
        if (rental == null)
            return NotFound(ApiResponse.FailResult("Đơn thuê không tồn tại"));

        return Ok(ApiResponse<RentalDetailResponse>.SuccessResult(rental));
    }

    /// <summary>
    /// Xác nhận đơn thuê
    /// </summary>
    [HttpPut("{id:guid}/confirm")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConfirmRental(Guid id)
    {
        var result = await _rentalService.ConfirmRentalAsync(id);
        if (!result)
            return BadRequest(ApiResponse.FailResult("Không thể xác nhận đơn thuê"));

        return Ok(ApiResponse.SuccessResult("Xác nhận đơn thuê thành công"));
    }

    /// <summary>
    /// Bắt đầu thuê xe
    /// </summary>
    [HttpPut("{id:guid}/start")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> StartRental(Guid id)
    {
        var result = await _rentalService.StartRentalAsync(id);
        if (!result)
            return BadRequest(ApiResponse.FailResult("Không thể bắt đầu thuê xe"));

        return Ok(ApiResponse.SuccessResult("Bắt đầu thuê xe thành công"));
    }

    /// <summary>
    /// Hoàn thành đơn thuê
    /// </summary>
    [HttpPut("{id:guid}/complete")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CompleteRental(Guid id, [FromQuery] DateTime? actualReturnDate = null)
    {
        var result = await _rentalService.CompleteRentalAsync(id, actualReturnDate);
        if (!result)
            return BadRequest(ApiResponse.FailResult("Không thể hoàn thành đơn thuê"));

        return Ok(ApiResponse.SuccessResult("Hoàn thành đơn thuê thành công"));
    }

    /// <summary>
    /// Hủy đơn thuê
    /// </summary>
    [HttpPut("{id:guid}/cancel")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CancelRental(Guid id, [FromBody] CancelRentalRequest request)
    {
        var result = await _rentalService.CancelRentalAsync(id, request.CancellationReason);
        if (!result)
            return BadRequest(ApiResponse.FailResult("Không thể hủy đơn thuê"));

        return Ok(ApiResponse.SuccessResult("Hủy đơn thuê thành công"));
    }

    /// <summary>
    /// Gia hạn đơn thuê
    /// </summary>
    [HttpPost("{id:guid}/extend")]
    [ProducesResponseType(typeof(ApiResponse<RentalDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ExtendRental(Guid id, [FromBody] ExtendRentalRequest request)
    {
        try
        {
            var rental = await _rentalService.ExtendRentalAsync(id, request);
            if (rental == null)
                return BadRequest(ApiResponse.FailResult("Không thể gia hạn đơn thuê"));

            return Ok(ApiResponse<RentalDetailResponse>.SuccessResult(rental, "Gia hạn thành công"));
        }
        catch (BusinessException ex)
        {
            return BadRequest(ApiResponse.FailResult(ex.Message));
        }
    }

    /// <summary>
    /// Cập nhật đơn thuê
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<RentalDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateRental(Guid id, [FromBody] UpdateRentalRequest request)
    {
        var rental = await _rentalService.UpdateRentalAsync(id, request);
        if (rental == null)
            return NotFound(ApiResponse.FailResult("Đơn thuê không tồn tại"));

        return Ok(ApiResponse<RentalDetailResponse>.SuccessResult(rental, "Cập nhật thành công"));
    }

    /// <summary>
    /// Lấy danh sách đơn thuê của khách hàng
    /// </summary>
    [HttpGet("my-rentals/{customerId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<RentalResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyRentals(Guid customerId, [FromQuery] RentalFilterCriteria criteria)
    {
        var rentals = await _rentalService.GetCustomerRentalsAsync(customerId, criteria);
        return Ok(ApiResponse<PagedResult<RentalResponse>>.SuccessResult(rentals));
    }

    /// <summary>
    /// Lấy danh sách đơn thuê của nhà cung cấp
    /// </summary>
    [HttpGet("supplier/{supplierId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<RentalResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSupplierRentals(Guid supplierId, [FromQuery] RentalFilterCriteria criteria)
    {
        var rentals = await _rentalService.GetSupplierRentalsAsync(supplierId, criteria);
        return Ok(ApiResponse<PagedResult<RentalResponse>>.SuccessResult(rentals));
    }

    /// <summary>
    /// Tìm kiếm đơn thuê (Admin)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<RentalResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchRentals([FromQuery] RentalFilterCriteria criteria)
    {
        var rentals = await _rentalService.SearchRentalsAsync(criteria);
        return Ok(ApiResponse<PagedResult<RentalResponse>>.SuccessResult(rentals));
    }

    /// <summary>
    /// Lấy các đơn thuê đang active
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<RentalResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActiveRentals()
    {
        var rentals = await _rentalService.GetActiveRentalsAsync();
        return Ok(ApiResponse<IEnumerable<RentalResponse>>.SuccessResult(rentals));
    }

    /// <summary>
    /// Tính giá thuê
    /// </summary>
    [HttpGet("calculate")]
    [ProducesResponseType(typeof(ApiResponse<decimal>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CalculateAmount(
        [FromQuery] Guid carId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        [FromQuery] string? couponCode = null)
    {
        var amount = await _rentalService.CalculateTotalAmountAsync(carId, startDate, endDate, couponCode);
        return Ok(ApiResponse<decimal>.SuccessResult(amount));
    }
}
