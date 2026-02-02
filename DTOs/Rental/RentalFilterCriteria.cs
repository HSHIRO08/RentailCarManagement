namespace RentailCarManagement.DTOs.Rental;

/// <summary>
/// Tiêu chí lọc đơn thuê
/// </summary>
public class RentalFilterCriteria
{
    /// <summary>
    /// ID khách hàng
    /// </summary>
    public Guid? CustomerId { get; set; }

    /// <summary>
    /// ID xe
    /// </summary>
    public Guid? CarId { get; set; }

    /// <summary>
    /// ID nhà cung cấp
    /// </summary>
    public Guid? SupplierId { get; set; }

    /// <summary>
    /// Trạng thái
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Từ ngày
    /// </summary>
    public DateTime? FromDate { get; set; }

    /// <summary>
    /// Đến ngày
    /// </summary>
    public DateTime? ToDate { get; set; }

    /// <summary>
    /// Số tiền tối thiểu
    /// </summary>
    public decimal? MinAmount { get; set; }

    /// <summary>
    /// Số tiền tối đa
    /// </summary>
    public decimal? MaxAmount { get; set; }

    /// <summary>
    /// Sắp xếp theo
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Thứ tự sắp xếp
    /// </summary>
    public string? SortOrder { get; set; } = "desc";

    /// <summary>
    /// Trang hiện tại
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Số item mỗi trang
    /// </summary>
    public int PageSize { get; set; } = 10;
}
