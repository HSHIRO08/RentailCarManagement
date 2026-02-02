using RentailCarManagement.Models;
using RentailCarManagement.DTOs.Car;

namespace RentailCarManagement.Repositories.Interfaces;

/// <summary>
/// Car Repository Interface
/// </summary>
public interface ICarRepository : IGenericRepository<Car>
{
    /// <summary>
    /// Lấy xe available (chưa được thuê)
    /// </summary>
    Task<IEnumerable<Car>> GetAvailableCarsAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Tìm kiếm xe theo tiêu chí
    /// </summary>
    Task<(IEnumerable<Car> Cars, int TotalCount)> SearchCarsAsync(CarSearchCriteria criteria);

    /// <summary>
    /// Lấy xe theo danh mục
    /// </summary>
    Task<IEnumerable<Car>> GetCarsByCategoryAsync(Guid categoryId);

    /// <summary>
    /// Lấy xe theo nhà cung cấp
    /// </summary>
    Task<IEnumerable<Car>> GetCarsBySupplierAsync(Guid supplierId);

    /// <summary>
    /// Kiểm tra xe có available trong khoảng thời gian
    /// </summary>
    Task<bool> IsCarAvailableAsync(Guid carId, DateTime startDate, DateTime endDate, Guid? excludeRentalId = null);

    /// <summary>
    /// Lấy xe với đầy đủ thông tin liên quan
    /// </summary>
    Task<Car?> GetCarWithDetailsAsync(Guid carId);

    /// <summary>
    /// Lấy danh sách xe phổ biến
    /// </summary>
    Task<IEnumerable<Car>> GetPopularCarsAsync(int count = 10);

    /// <summary>
    /// Lấy rating trung bình của xe
    /// </summary>
    Task<decimal?> GetAverageRatingAsync(Guid carId);
}
