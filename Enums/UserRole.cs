namespace RentailCarManagement.Enums;

/// <summary>
/// Vai trò người dùng trong hệ thống
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Quản trị viên
    /// </summary>
    Admin = 0,

    /// <summary>
    /// Khách hàng
    /// </summary>
    Customer = 1,

    /// <summary>
    /// Nhà cung cấp/Chủ xe
    /// </summary>
    Supplier = 2,

    /// <summary>
    /// Nhân viên
    /// </summary>
    Staff = 3
}
