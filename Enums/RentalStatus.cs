namespace RentailCarManagement.Enums;

/// <summary>
/// Trạng thái của đơn thuê xe
/// </summary>
public enum RentalStatus
{
    /// <summary>
    /// Đang chờ xác nhận
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Đã xác nhận
    /// </summary>
    Confirmed = 1,

    /// <summary>
    /// Đang thuê
    /// </summary>
    Active = 2,

    /// <summary>
    /// Đã hoàn thành
    /// </summary>
    Completed = 3,

    /// <summary>
    /// Đã hủy
    /// </summary>
    Cancelled = 4
}
