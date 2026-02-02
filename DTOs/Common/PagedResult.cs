namespace RentailCarManagement.DTOs.Common;

/// <summary>
/// DTO phân trang kết quả
/// </summary>
/// <typeparam name="T">Loại dữ liệu trong danh sách</typeparam>
public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalItems { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
