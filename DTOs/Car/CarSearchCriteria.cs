namespace RentailCarManagement.DTOs.Car;

/// <summary>
/// Tiêu chí tìm kiếm xe
/// </summary>
public class CarSearchCriteria
{
    /// <summary>
    /// Từ khóa tìm kiếm (brand, model, license plate)
    /// </summary>
    public string? Keyword { get; set; }

    /// <summary>
    /// ID danh mục xe
    /// </summary>
    public Guid? CategoryId { get; set; }

    /// <summary>
    /// Giá tối thiểu
    /// </summary>
    public decimal? MinPrice { get; set; }

    /// <summary>
    /// Giá tối đa
    /// </summary>
    public decimal? MaxPrice { get; set; }

    /// <summary>
    /// Loại nhiên liệu
    /// </summary>
    public string? FuelType { get; set; }

    /// <summary>
    /// Loại hộp số
    /// </summary>
    public string? Transmission { get; set; }

    /// <summary>
    /// Số ghế tối thiểu
    /// </summary>
    public int? MinSeats { get; set; }

    /// <summary>
    /// Số ghế tối đa
    /// </summary>
    public int? MaxSeats { get; set; }

    /// <summary>
    /// Năm sản xuất từ
    /// </summary>
    public int? YearFrom { get; set; }

    /// <summary>
    /// Năm sản xuất đến
    /// </summary>
    public int? YearTo { get; set; }

    /// <summary>
    /// Vị trí
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// Ngày bắt đầu thuê (để kiểm tra availability)
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Ngày kết thúc thuê
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// ID nhà cung cấp
    /// </summary>
    public Guid? SupplierId { get; set; }

    /// <summary>
    /// Chỉ lấy xe đã được duyệt
    /// </summary>
    public bool? IsApproved { get; set; }

    /// <summary>
    /// Trạng thái xe
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Sắp xếp theo (price, rating, year, createdAt)
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Thứ tự sắp xếp (asc, desc)
    /// </summary>
    public string? SortOrder { get; set; } = "asc";

    /// <summary>
    /// Trang hiện tại
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Số item mỗi trang
    /// </summary>
    public int PageSize { get; set; } = 10;
}
