using Microsoft.AspNetCore.Mvc;
using RentailCarManagement.DTOs.Payment;
using RentailCarManagement.DTOs.Common;
using RentailCarManagement.Services.Interfaces;
using RentailCarManagement.Exceptions;

namespace RentailCarManagement.Controllers;

/// <summary>
/// API Controller cho quản lý thanh toán
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    /// <summary>
    /// Xử lý thanh toán
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PaymentResponse>), StatusCodes.Status201Created)]
    public async Task<IActionResult> ProcessPayment([FromBody] CreatePaymentRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse.FailResult("Dữ liệu không hợp lệ"));

        try
        {
            var payment = await _paymentService.ProcessPaymentAsync(request);
            return CreatedAtAction(nameof(GetPaymentDetails), new { id = payment.PaymentId },
                ApiResponse<PaymentResponse>.SuccessResult(payment, "Xử lý thanh toán thành công"));
        }
        catch (BusinessException ex)
        {
            return BadRequest(ApiResponse.FailResult(ex.Message));
        }
    }

    /// <summary>
    /// Lấy chi tiết thanh toán
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PaymentResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaymentDetails(Guid id)
    {
        var payment = await _paymentService.GetPaymentDetailsAsync(id);
        if (payment == null)
            return NotFound(ApiResponse.FailResult("Thanh toán không tồn tại"));

        return Ok(ApiResponse<PaymentResponse>.SuccessResult(payment));
    }

    /// <summary>
    /// Lấy thanh toán của đơn thuê
    /// </summary>
    [HttpGet("rental/{rentalId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PaymentResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaymentByRental(Guid rentalId)
    {
        var payment = await _paymentService.GetPaymentByRentalAsync(rentalId);
        if (payment == null)
            return NotFound(ApiResponse.FailResult("Không tìm thấy thanh toán cho đơn thuê này"));

        return Ok(ApiResponse<PaymentResponse>.SuccessResult(payment));
    }

    /// <summary>
    /// Xử lý hoàn tiền
    /// </summary>
    [HttpPost("{id:guid}/refund")]
    [ProducesResponseType(typeof(ApiResponse<PaymentResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ProcessRefund(Guid id, [FromBody] RefundRequest request)
    {
        try
        {
            var payment = await _paymentService.ProcessRefundAsync(id, request);
            if (payment == null)
                return NotFound(ApiResponse.FailResult("Thanh toán không tồn tại"));

            return Ok(ApiResponse<PaymentResponse>.SuccessResult(payment, "Hoàn tiền thành công"));
        }
        catch (BusinessException ex)
        {
            return BadRequest(ApiResponse.FailResult(ex.Message));
        }
    }

    /// <summary>
    /// Xác nhận thanh toán
    /// </summary>
    [HttpPut("{id:guid}/verify")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> VerifyPayment(Guid id, [FromBody] string transactionId)
    {
        var result = await _paymentService.VerifyPaymentAsync(id, transactionId);
        if (!result)
            return NotFound(ApiResponse.FailResult("Thanh toán không tồn tại"));

        return Ok(ApiResponse.SuccessResult("Xác nhận thanh toán thành công"));
    }

    /// <summary>
    /// Lấy lịch sử thanh toán của khách hàng
    /// </summary>
    [HttpGet("customer/{customerId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<PaymentResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaymentHistory(Guid customerId)
    {
        var payments = await _paymentService.GetPaymentHistoryAsync(customerId);
        return Ok(ApiResponse<IEnumerable<PaymentResponse>>.SuccessResult(payments));
    }

    /// <summary>
    /// Lấy các thanh toán đang chờ
    /// </summary>
    [HttpGet("pending")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<PaymentResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPendingPayments()
    {
        var payments = await _paymentService.GetPendingPaymentsAsync();
        return Ok(ApiResponse<IEnumerable<PaymentResponse>>.SuccessResult(payments));
    }

    /// <summary>
    /// Lấy tổng doanh thu
    /// </summary>
    [HttpGet("revenue")]
    [ProducesResponseType(typeof(ApiResponse<decimal>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTotalRevenue([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
    {
        var revenue = await _paymentService.GetTotalRevenueAsync(fromDate, toDate);
        return Ok(ApiResponse<decimal>.SuccessResult(revenue));

    }
}
