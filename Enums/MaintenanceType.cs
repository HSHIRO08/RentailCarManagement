namespace RentailCarManagement.Enums;

/// <summary>
/// Loại bảo trì
/// </summary>
public enum MaintenanceType
{
    /// <summary>
    /// Bảo dưỡng định kỳ
    /// </summary>
    Scheduled = 0,

    /// <summary>
    /// Sửa chữa
    /// </summary>
    Repair = 1,

    /// <summary>
    /// Thay thế phụ tùng
    /// </summary>
    PartReplacement = 2,

    /// <summary>
    /// Kiểm tra
    /// </summary>
    Inspection = 3
}
