using RentailCarManagement.DTOs.Payment;
using RentailCarManagement.Models;
using RentailCarManagement.Repositories.Interfaces;
using RentailCarManagement.Services.Interfaces;
using RentailCarManagement.Exceptions;

namespace RentailCarManagement.Services.Implementations;

/// <summary>
/// Payment Service Implementation
/// </summary>
public class PaymentService : IPaymentService
{
    private readonly IUnitOfWork _unitOfWork;

    public PaymentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaymentResponse> ProcessPaymentAsync(CreatePaymentRequest request)
    {
        // Validate rental exists
        var rental = await _unitOfWork.Rentals.GetRentalWithDetailsAsync(request.RentalId);
        if (rental == null)
            throw new NotFoundException("Đơn thuê không tồn tại");

        // Check if payment already exists
        var existingPayment = await _unitOfWork.Payments.GetPaymentByRentalAsync(request.RentalId);
        if (existingPayment != null)
            throw new BusinessException("Đơn thuê này đã có thanh toán");

        var payment = new Payment
        {
            PaymentId = Guid.NewGuid(),
            RentalId = request.RentalId,
            Amount = request.Amount,
            PaymentMethod = request.PaymentMethod,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Payments.AddAsync(payment);
        await _unitOfWork.SaveChangesAsync();

        return MapToPaymentResponse(payment, rental);
    }

    public async Task<PaymentResponse?> GetPaymentDetailsAsync(Guid paymentId)
    {
        var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId);
        if (payment == null)
            return null;

        var rental = await _unitOfWork.Rentals.GetRentalWithDetailsAsync(payment.RentalId);
        return MapToPaymentResponse(payment, rental);
    }

    public async Task<PaymentResponse?> GetPaymentByRentalAsync(Guid rentalId)
    {
        var payment = await _unitOfWork.Payments.GetPaymentByRentalAsync(rentalId);
        if (payment == null)
            return null;

        return MapToPaymentResponse(payment, payment.Rental);
    }

    public async Task<PaymentResponse?> ProcessRefundAsync(Guid paymentId, RefundRequest request)
    {
        var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId);
        if (payment == null)
            return null;

        if (payment.Status != "Paid")
            throw new BusinessException("Chỉ có thể hoàn tiền cho thanh toán đã thành công");

        if (request.Amount > payment.Amount)
            throw new BusinessException("Số tiền hoàn không thể lớn hơn số tiền đã thanh toán");

        payment.Status = "Refunded";
        _unitOfWork.Payments.Update(payment);
        await _unitOfWork.SaveChangesAsync();

        var rental = await _unitOfWork.Rentals.GetRentalWithDetailsAsync(payment.RentalId);
        return MapToPaymentResponse(payment, rental);
    }

    public async Task<bool> VerifyPaymentAsync(Guid paymentId, string transactionId)
    {
        var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId);
        if (payment == null)
            return false;

        payment.Status = "Paid";
        payment.PaidAt = DateTime.UtcNow;

        _unitOfWork.Payments.Update(payment);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<PaymentResponse>> GetPaymentHistoryAsync(Guid customerId)
    {
        var payments = await _unitOfWork.Payments.GetPaymentsByCustomerAsync(customerId);
        return payments.Select(p => MapToPaymentResponse(p, p.Rental));
    }

    public async Task<IEnumerable<PaymentResponse>> GetPendingPaymentsAsync()
    {
        var payments = await _unitOfWork.Payments.GetPendingPaymentsAsync();
        return payments.Select(p => MapToPaymentResponse(p, p.Rental));
    }

    public async Task<decimal> GetTotalRevenueAsync(DateTime fromDate, DateTime toDate)
    {
        return await _unitOfWork.Payments.GetTotalRevenueAsync(fromDate, toDate);
    }

    private PaymentResponse MapToPaymentResponse(Payment payment, Rental? rental)
    {
        return new PaymentResponse
        {
            PaymentId = payment.PaymentId,
            RentalId = payment.RentalId,
            Amount = payment.Amount,
            PaymentMethod = payment.PaymentMethod,
            Status = payment.Status,
            PaidAt = payment.PaidAt,
            CreatedAt = payment.CreatedAt,
            CarInfo = rental?.Car != null ? $"{rental.Car.Brand} {rental.Car.Model}" : null,
            CustomerName = rental?.Customer?.User?.FullName
        };
    }
}
