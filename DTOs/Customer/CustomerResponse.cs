namespace RentailCarManagement.DTOs.Customer;

/// <summary>
/// DTO trả về thông tin khách hàng
/// </summary>
public class CustomerResponse
{
    public Guid CustomerId { get; set; }
    public Guid UserId { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? DriverLicenseNumber { get; set; }
    public DateOnly? LicenseExpiryDate { get; set; }
    public string? Address { get; set; }
    public DateTime? CreatedAt { get; set; }

    // Statistics
    public int TotalRentals { get; set; }
    public int CompletedRentals { get; set; }
    public decimal TotalSpent { get; set; }
}
