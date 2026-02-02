namespace RentailCarManagement.Enums;

/// <summary>
/// Phương thức thanh toán
/// </summary>
public enum PaymentMethod
{
    /// <summary>
    /// Tiền mặt
    /// </summary>
    Cash = 0,

    /// <summary>
    /// Thẻ tín dụng
    /// </summary>
    CreditCard = 1,

    /// <summary>
    /// Thẻ ghi nợ
    /// </summary>
    DebitCard = 2,

    /// <summary>
    /// Chuyển khoản ngân hàng
    /// </summary>
    BankTransfer = 3,

    /// <summary>
    /// Ví điện tử
    /// </summary>
    Wallet = 4
}
