using RentailCarManagement.Models;

namespace RentailCarManagement.Repositories.Interfaces;

/// <summary>
/// Customer Repository Interface
/// </summary>
public interface ICustomerRepository : IGenericRepository<Customer>
{
    /// <summary>
    /// Lấy khách hàng theo UserId
    /// </summary>
    Task<Customer?> GetCustomerByUserIdAsync(Guid userId);

    /// <summary>
    /// Lấy khách hàng với lịch sử thuê xe
    /// </summary>
    Task<Customer?> GetCustomerWithRentalsAsync(Guid customerId);

    /// <summary>
    /// Lấy thông tin loyalty của khách hàng
    /// </summary>
    Task<(int TotalRentals, decimal TotalSpent)> GetCustomerStatsAsync(Guid customerId);

    /// <summary>
    /// Kiểm tra giấy phép lái xe còn hạn
    /// </summary>
    Task<bool> IsLicenseValidAsync(Guid customerId, DateTime rentalDate);
}
