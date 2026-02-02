using RentailCarManagement.Models;
using RentailCarManagement.DTOs.Rental;

namespace RentailCarManagement.Repositories.Interfaces;

/// <summary>
/// Rental Repository Interface
/// </summary>
public interface IRentalRepository : IGenericRepository<Rental>
{
    /// <summary>
    /// Lấy các đơn thuê đang active
    /// </summary>
    Task<IEnumerable<Rental>> GetActiveRentalsAsync();

    /// <summary>
    /// Lấy lịch sử thuê của khách hàng
    /// </summary>
    Task<IEnumerable<Rental>> GetRentalsByCustomerAsync(Guid customerId);

    /// <summary>
    /// Lấy các đơn thuê của xe
    /// </summary>
    Task<IEnumerable<Rental>> GetRentalsByCarAsync(Guid carId);

    /// <summary>
    /// Lấy các đơn thuê của nhà cung cấp
    /// </summary>
    Task<IEnumerable<Rental>> GetRentalsBySupplierAsync(Guid supplierId);

    /// <summary>
    /// Tìm kiếm đơn thuê theo tiêu chí
    /// </summary>
    Task<(IEnumerable<Rental> Rentals, int TotalCount)> SearchRentalsAsync(RentalFilterCriteria criteria);

    /// <summary>
    /// Lấy đơn thuê với đầy đủ thông tin
    /// </summary>
    Task<Rental?> GetRentalWithDetailsAsync(Guid rentalId);

    /// <summary>
    /// Kiểm tra có xung đột lịch thuê không
    /// </summary>
    Task<bool> HasScheduleConflictAsync(Guid carId, DateTime startDate, DateTime endDate, Guid? excludeRentalId = null);

    /// <summary>
    /// Lấy các đơn thuê sắp hết hạn
    /// </summary>
    Task<IEnumerable<Rental>> GetExpiringRentalsAsync(int daysFromNow = 1);

    /// <summary>
    /// Lấy các đơn thuê quá hạn
    /// </summary>
    Task<IEnumerable<Rental>> GetOverdueRentalsAsync();

    /// <summary>
    /// Thống kê doanh thu theo khoảng thời gian
    /// </summary>
    Task<decimal> GetRevenueAsync(DateTime fromDate, DateTime toDate, Guid? supplierId = null);
}
