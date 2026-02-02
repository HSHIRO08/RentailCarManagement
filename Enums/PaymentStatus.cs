namespace RentailCarManagement.Enums;

/// <summary>
/// Trạng thái thanh toán
/// </summary>
public enum PaymentStatus
{
    /// <summary>
    /// Chưa thanh toán
    /// </summary>
    Unpaid = 0,

    /// <summary>
    /// Thanh toán một phần
    /// </summary>
    Partial = 1,

    /// <summary>
    /// Đã thanh toán
    /// </summary>
    Paid = 2,

    /// <summary>
    /// Đã hoàn tiền
    /// </summary>
    Refunded = 3
}
