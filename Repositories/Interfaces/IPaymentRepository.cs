using RentailCarManagement.Models;

namespace RentailCarManagement.Repositories.Interfaces;

/// <summary>
/// Payment Repository Interface
/// </summary>
public interface IPaymentRepository : IGenericRepository<Payment>
{
    /// <summary>
    /// Lấy thanh toán của đơn thuê
    /// </summary>
    Task<Payment?> GetPaymentByRentalAsync(Guid rentalId);

    /// <summary>
    /// Lấy lịch sử thanh toán của khách hàng
    /// </summary>
    Task<IEnumerable<Payment>> GetPaymentsByCustomerAsync(Guid customerId);

    /// <summary>
    /// Lấy các thanh toán theo trạng thái
    /// </summary>
    Task<IEnumerable<Payment>> GetPaymentsByStatusAsync(string status);

    /// <summary>
    /// Lấy doanh thu theo khoảng thời gian
    /// </summary>
    Task<decimal> GetTotalRevenueAsync(DateTime fromDate, DateTime toDate);

    /// <summary>
    /// Lấy các thanh toán chưa hoàn tất
    /// </summary>
    Task<IEnumerable<Payment>> GetPendingPaymentsAsync();
}
