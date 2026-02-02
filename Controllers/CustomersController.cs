using Microsoft.AspNetCore.Mvc;
using RentailCarManagement.DTOs.Customer;
using RentailCarManagement.DTOs.Common;
using RentailCarManagement.Services.Interfaces;

namespace RentailCarManagement.Controllers;

/// <summary>
/// API Controller cho quản lý khách hàng
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(ICustomerService customerService, ILogger<CustomersController> logger)
    {
        _customerService = customerService;
        _logger = logger;
    }

    /// <summary>
    /// Lấy thông tin khách hàng
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CustomerResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCustomerProfile(Guid id)
    {
        var customer = await _customerService.GetCustomerProfileAsync(id);
        if (customer == null)
            return NotFound(ApiResponse.FailResult("Khách hàng không tồn tại"));

        return Ok(ApiResponse<CustomerResponse>.SuccessResult(customer));
    }

    /// <summary>
    /// Lấy khách hàng theo UserId
    /// </summary>
    [HttpGet("user/{userId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CustomerResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCustomerByUserId(Guid userId)
    {
        var customer = await _customerService.GetCustomerByUserIdAsync(userId);
        if (customer == null)
            return NotFound(ApiResponse.FailResult("Khách hàng không tồn tại"));

        return Ok(ApiResponse<CustomerResponse>.SuccessResult(customer));
    }

    /// <summary>
    /// Cập nhật thông tin khách hàng
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CustomerResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProfile(Guid id, [FromBody] UpdateCustomerRequest request)
    {
        var customer = await _customerService.UpdateCustomerProfileAsync(id, request);
        if (customer == null)
            return NotFound(ApiResponse.FailResult("Khách hàng không tồn tại"));

        return Ok(ApiResponse<CustomerResponse>.SuccessResult(customer, "Cập nhật thành công"));
    }

    /// <summary>
    /// Lấy thống kê khách hàng
    /// </summary>
    [HttpGet("{id:guid}/stats")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCustomerStats(Guid id)
    {
        var stats = await _customerService.GetCustomerStatsAsync(id);
        return Ok(ApiResponse<object>.SuccessResult(new
        {
            stats.TotalRentals,
            stats.CompletedRentals,
            stats.TotalSpent
        }));
    }

    /// <summary>
    /// Kiểm tra giấy phép lái xe
    /// </summary>
    [HttpGet("{id:guid}/validate-license")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ValidateDriverLicense(Guid id, [FromQuery] DateTime rentalDate)
    {
        var isValid = await _customerService.ValidateDriverLicenseAsync(id, rentalDate);
        return Ok(ApiResponse<bool>.SuccessResult(isValid));
    }
}
