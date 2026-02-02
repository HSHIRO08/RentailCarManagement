namespace RentailCarManagement.Enums;

/// <summary>
/// Trạng thái khiếu nại
/// </summary>
public enum ComplaintStatus
{
    /// <summary>
    /// Mở
    /// </summary>
    Open = 0,

    /// <summary>
    /// Đang xử lý
    /// </summary>
    InProgress = 1,

    /// <summary>
    /// Đã giải quyết
    /// </summary>
    Resolved = 2,

    /// <summary>
    /// Đã đóng
    /// </summary>
    Closed = 3
}
