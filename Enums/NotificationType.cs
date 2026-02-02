namespace RentailCarManagement.Enums;

/// <summary>
/// Loại thông báo
/// </summary>
public enum NotificationType
{
    /// <summary>
    /// Thông báo đặt xe
    /// </summary>
    Booking = 0,

    /// <summary>
    /// Thông báo thanh toán
    /// </summary>
    Payment = 1,

    /// <summary>
    /// Thông báo đánh giá
    /// </summary>
    Review = 2,

    /// <summary>
    /// Thông báo khuyến mãi
    /// </summary>
    Promotion = 3,

    /// <summary>
    /// Thông báo hệ thống
    /// </summary>
    System = 4
}
