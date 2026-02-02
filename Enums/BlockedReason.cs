namespace RentailCarManagement.Enums;

/// <summary>
/// Lý do chặn ngày
/// </summary>
public enum BlockedReason
{
    /// <summary>
    /// Bảo trì
    /// </summary>
    Maintenance = 0,

    /// <summary>
    /// Đã đặt trước
    /// </summary>
    Reserved = 1,

    /// <summary>
    /// Ngày lễ
    /// </summary>
    Holiday = 2,

    /// <summary>
    /// Lý do khác
    /// </summary>
    Other = 3
}
