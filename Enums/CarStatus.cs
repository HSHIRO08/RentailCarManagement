namespace RentailCarManagement.Enums;

/// <summary>
/// Trạng thái của xe
/// </summary>
public enum CarStatus
{
    /// <summary>
    /// Xe sẵn sàng cho thuê
    /// </summary>
    Available = 0,

    /// <summary>
    /// Xe đang được thuê
    /// </summary>
    Rented = 1,

    /// <summary>
    /// Xe đang bảo trì
    /// </summary>
    Maintenance = 2,

    /// <summary>
    /// Xe bị hư hỏng
    /// </summary>
    Damaged = 3,

    /// <summary>
    /// Xe đã ngừng hoạt động
    /// </summary>
    Retired = 4
}
