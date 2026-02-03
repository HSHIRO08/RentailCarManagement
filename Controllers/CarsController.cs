using Microsoft.AspNetCore.Mvc;
using RentailCarManagement.DTOs.Car;
using RentailCarManagement.DTOs.Common;
using RentailCarManagement.Services.Interfaces;

namespace RentailCarManagement.Controllers;

/// <summary>
/// API Controller cho quản lý xe
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CarsController : ControllerBase
{
    private readonly ICarService _carService;
    private readonly ILogger<CarsController> _logger;

    public CarsController(ICarService carService, ILogger<CarsController> logger)
    {
        _carService = carService;
        _logger = logger;
    }

    /// <summary>
    /// Tìm kiếm xe với các bộ lọc
    /// </summary>
    /// <param name="criteria">Tiêu chí tìm kiếm</param>
    /// <returns>Danh sách xe phù hợp</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<CarResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchCars([FromQuery] CarSearchCriteria criteria)
    {
        var result = await _carService.SearchAvailableCarsAsync(criteria);
        return Ok(ApiResponse<PagedResult<CarResponse>>.SuccessResult(result));
    }

    /// <summary>
    /// Lấy chi tiết xe
    /// </summary>
    /// <param name="id">ID xe</param>
    /// <returns>Thông tin chi tiết xe</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CarDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCarDetails(Guid id)
    {
        var car = await _carService.GetCarDetailsAsync(id);
        if (car == null)
            return NotFound(ApiResponse.FailResult("Xe không tồn tại"));

        return Ok(ApiResponse<CarDetailResponse>.SuccessResult(car));
    }

    /// <summary>
    /// Kiểm tra xe có khả dụng trong khoảng thời gian
    /// </summary>
    [HttpGet("{id:guid}/availability")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckAvailability(Guid id, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var isAvailable = await _carService.CheckCarAvailabilityAsync(id, startDate, endDate);
        return Ok(ApiResponse<bool>.SuccessResult(isAvailable));
    }

    /// <summary>
    /// Lấy danh sách xe khả dụng
    /// </summary>
    [HttpGet("available")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<CarResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAvailableCars([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] CarSearchCriteria criteria)
    {
        criteria.StartDate = startDate;
        criteria.EndDate = endDate;
        criteria.Status = "Available";
        criteria.IsApproved = true;

        var result = await _carService.SearchAvailableCarsAsync(criteria);
        return Ok(ApiResponse<PagedResult<CarResponse>>.SuccessResult(result));
    }

    /// <summary>
    /// Lấy danh sách danh mục xe
    /// </summary>
    [HttpGet("categories")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CarCategoryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _carService.GetCategoriesAsync();
        return Ok(ApiResponse<IEnumerable<CarCategoryDto>>.SuccessResult(categories));
    }

    /// <summary>
    /// Lấy danh sách xe phổ biến
    /// </summary>
    [HttpGet("popular")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CarResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPopularCars([FromQuery] int count = 10)
    {
        var cars = await _carService.GetPopularCarsAsync(count);
        return Ok(ApiResponse<IEnumerable<CarResponse>>.SuccessResult(cars));
    }

    /// <summary>
    /// Tạo xe mới (Supplier/Admin)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CarResponse>), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateCar([FromBody] CreateCarRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse.FailResult("Dữ liệu không hợp lệ"));

        var car = await _carService.CreateCarAsync(request);
        return CreatedAtAction(nameof(GetCarDetails), new { id = car.CarId }, 
            ApiResponse<CarResponse>.SuccessResult(car, "Tạo xe thành công"));
    }

    /// <summary>
    /// Cập nhật thông tin xe
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CarResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateCar(Guid id, [FromBody] UpdateCarRequest request)
    {
        var car = await _carService.UpdateCarAsync(id, request);
        if (car == null)
            return NotFound(ApiResponse.FailResult("Xe không tồn tại"));

        return Ok(ApiResponse<CarResponse>.SuccessResult(car, "Cập nhật xe thành công"));
    }

    /// <summary>
    /// Xóa xe (soft delete)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteCar(Guid id)
    {
        var result = await _carService.DeleteCarAsync(id);
        if (!result)
            return BadRequest(ApiResponse.FailResult("Không thể xóa xe. Xe có thể đang được thuê hoặc không tồn tại."));

        return Ok(ApiResponse.SuccessResult("Xóa xe thành công"));
    }

    /// <summary>
    /// Cập nhật trạng thái xe
    /// </summary>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] string status)
    {
        var result = await _carService.UpdateCarStatusAsync(id, status);
        if (!result)
            return NotFound(ApiResponse.FailResult("Xe không tồn tại"));

        return Ok(ApiResponse.SuccessResult("Cập nhật trạng thái thành công"));
    }

    /// <summary>
    /// Tính giá thuê xe
    /// </summary>
    [HttpGet("{id:guid}/calculate-price")]
    [ProducesResponseType(typeof(ApiResponse<decimal>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CalculatePrice(Guid id, [FromQuery] int days)
    {
        var price = await _carService.CalculateRentalPriceAsync(id, days);
        return Ok(ApiResponse<decimal>.SuccessResult(price));
    }

    /// <summary>
    /// Lấy xe theo nhà cung cấp
    /// </summary>
    [HttpGet("supplier/{supplierId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CarResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCarsBySupplier(Guid supplierId)
    {
        var cars = await _carService.GetCarsBySupplierAsync(supplierId);
        return Ok(ApiResponse<IEnumerable<CarResponse>>.SuccessResult(cars));
    }
}
