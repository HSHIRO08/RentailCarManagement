namespace RentailCarManagement.Enums;

/// <summary>
/// Trạng thái phê duyệt
/// </summary>
public enum ApprovalStatus
{
    /// <summary>
    /// Đang chờ
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Đã phê duyệt
    /// </summary>
    Approved = 1,

    /// <summary>
    /// Từ chối
    /// </summary>
    Rejected = 2
}
