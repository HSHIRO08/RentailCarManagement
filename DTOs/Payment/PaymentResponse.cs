namespace RentailCarManagement.DTOs.Payment;

/// <summary>
/// DTO trả về thông tin thanh toán
/// </summary>
public class PaymentResponse
{
    public Guid PaymentId { get; set; }
    public Guid RentalId { get; set; }
    public decimal Amount { get; set; }
    public string? PaymentMethod { get; set; }
    public string? PaymentType { get; set; }
    public string? Status { get; set; }
    public string? TransactionId { get; set; }
    public string? PaymentGateway { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? Notes { get; set; }

    // Related data
    public string? CarInfo { get; set; }
    public string? CustomerName { get; set; }
}
