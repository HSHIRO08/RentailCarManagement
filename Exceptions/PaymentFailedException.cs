namespace RentailCarManagement.Exceptions;

/// <summary>
/// Exception khi thanh toán thất bại
/// </summary>
public class PaymentFailedException : BusinessException
{
    public string? TransactionId { get; }
    public string? ErrorCode { get; }

    public PaymentFailedException(string message) : base(message)
    {
    }

    public PaymentFailedException(string message, string? transactionId, string? errorCode)
        : base(message)
    {
        TransactionId = transactionId;
        ErrorCode = errorCode;
    }
}
