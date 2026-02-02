using RentailCarManagement.DTOs.Customer;

namespace RentailCarManagement.Services.Interfaces;

/// <summary>
/// Customer Service Interface
/// </summary>
public interface ICustomerService
{
    /// <summary>
    /// Lấy thông tin khách hàng
    /// </summary>
    Task<CustomerResponse?> GetCustomerProfileAsync(Guid customerId);

    /// <summary>
    /// Lấy khách hàng theo UserId
    /// </summary>
    Task<CustomerResponse?> GetCustomerByUserIdAsync(Guid userId);

    /// <summary>
    /// Cập nhật thông tin khách hàng
    /// </summary>
    Task<CustomerResponse?> UpdateCustomerProfileAsync(Guid customerId, UpdateCustomerRequest request);

    /// <summary>
    /// Lấy thống kê khách hàng
    /// </summary>
    Task<(int TotalRentals, int CompletedRentals, decimal TotalSpent)> GetCustomerStatsAsync(Guid customerId);

    /// <summary>
    /// Kiểm tra giấy phép lái xe
    /// </summary>
    Task<bool> ValidateDriverLicenseAsync(Guid customerId, DateTime rentalDate);
}
