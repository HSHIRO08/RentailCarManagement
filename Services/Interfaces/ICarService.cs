using RentailCarManagement.DTOs.Car;
using RentailCarManagement.DTOs.Common;

namespace RentailCarManagement.Services.Interfaces;

/// <summary>
/// Car Service Interface
/// </summary>
public interface ICarService
{
    /// <summary>
    /// Tìm kiếm xe available
    /// </summary>
    Task<PagedResult<CarResponse>> SearchAvailableCarsAsync(CarSearchCriteria criteria);

    /// <summary>
    /// Lấy chi tiết xe
    /// </summary>
    Task<CarDetailResponse?> GetCarDetailsAsync(Guid carId);

    /// <summary>
    /// Kiểm tra xe có available không
    /// </summary>
    Task<bool> CheckCarAvailabilityAsync(Guid carId, DateTime startDate, DateTime endDate);

    /// <summary>
    /// Tạo xe mới
    /// </summary>
    Task<CarResponse> CreateCarAsync(CreateCarRequest request);

    /// <summary>
    /// Cập nhật xe
    /// </summary>
    Task<CarResponse?> UpdateCarAsync(Guid carId, UpdateCarRequest request);

    /// <summary>
    /// Xóa xe (soft delete)
    /// </summary>
    Task<bool> DeleteCarAsync(Guid carId);

    /// <summary>
    /// Cập nhật trạng thái xe
    /// </summary>
    Task<bool> UpdateCarStatusAsync(Guid carId, string status);

    /// <summary>
    /// Tính giá thuê
    /// </summary>
    Task<decimal> CalculateRentalPriceAsync(Guid carId, int days);

    /// <summary>
    /// Lấy danh sách danh mục xe
    /// </summary>
    Task<IEnumerable<CarCategoryDto>> GetCategoriesAsync();

    /// <summary>
    /// Lấy xe theo nhà cung cấp
    /// </summary>
    Task<IEnumerable<CarResponse>> GetCarsBySupplierAsync(Guid supplierId);

    /// <summary>
    /// Lấy xe phổ biến
    /// </summary>
    Task<IEnumerable<CarResponse>> GetPopularCarsAsync(int count = 10);
}
