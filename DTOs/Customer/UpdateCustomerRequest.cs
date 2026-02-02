using System.ComponentModel.DataAnnotations;

namespace RentailCarManagement.DTOs.Customer;

/// <summary>
/// DTO để cập nhật thông tin khách hàng
/// </summary>
public class UpdateCustomerRequest
{
    [StringLength(100)]
    public string? FullName { get; set; }

    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    public string? Phone { get; set; }

    [StringLength(50)]
    public string? DriverLicenseNumber { get; set; }

    public DateOnly? LicenseExpiryDate { get; set; }

    [StringLength(500)]
    public string? Address { get; set; }
}
