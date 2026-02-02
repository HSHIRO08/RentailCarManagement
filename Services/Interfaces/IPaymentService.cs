using RentailCarManagement.DTOs.Payment;
using RentailCarManagement.DTOs.Common;

namespace RentailCarManagement.Services.Interfaces;

/// <summary>
/// Payment Service Interface
/// </summary>
public interface IPaymentService
{
    /// <summary>
    /// Xử lý thanh toán
    /// </summary>
    Task<PaymentResponse> ProcessPaymentAsync(CreatePaymentRequest request);

    /// <summary>
    /// Lấy chi tiết thanh toán
    /// </summary>
    Task<PaymentResponse?> GetPaymentDetailsAsync(Guid paymentId);

    /// <summary>
    /// Lấy thanh toán của đơn thuê
    /// </summary>
    Task<PaymentResponse?> GetPaymentByRentalAsync(Guid rentalId);

    /// <summary>
    /// Xử lý hoàn tiền
    /// </summary>
    Task<PaymentResponse?> ProcessRefundAsync(Guid paymentId, RefundRequest request);

    /// <summary>
    /// Xác nhận thanh toán
    /// </summary>
    Task<bool> VerifyPaymentAsync(Guid paymentId, string transactionId);

    /// <summary>
    /// Lấy lịch sử thanh toán của khách hàng
    /// </summary>
    Task<IEnumerable<PaymentResponse>> GetPaymentHistoryAsync(Guid customerId);

    /// <summary>
    /// Lấy các thanh toán đang chờ
    /// </summary>
    Task<IEnumerable<PaymentResponse>> GetPendingPaymentsAsync();

    /// <summary>
    /// Lấy tổng doanh thu
    /// </summary>
    Task<decimal> GetTotalRevenueAsync(DateTime fromDate, DateTime toDate);
}
