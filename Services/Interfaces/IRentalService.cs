using RentailCarManagement.DTOs.Rental;
using RentailCarManagement.DTOs.Common;

namespace RentailCarManagement.Services.Interfaces;

/// <summary>
/// Rental Service Interface
/// </summary>
public interface IRentalService
{
    /// <summary>
    /// Tạo đơn thuê mới
    /// </summary>
    Task<RentalDetailResponse> Create(CreateRentalRequest request);

    /// <summary>
    /// Lấy chi tiết đơn thuê
    /// </summary>
    Task<RentalDetailResponse?> Get(Guid rentalId);

    /// <summary>
    /// Xác nhận đơn thuê
    /// </summary>
    Task<bool> ConfirmRentalAsync(Guid rentalId);

    /// <summary>
    /// Bắt đầu thuê xe
    /// </summary>
    Task<bool> Start(Guid rentalId);

    /// <summary>
    /// Hoàn thành đơn thuê
    /// </summary>
    Task<bool> Complete(Guid rentalId, DateTime? actualReturnDate = null);

    /// <summary>
    /// Hủy đơn thuê
    /// </summary>
    Task<bool> Cancel(Guid rentalId, string reason);

    /// <summary>
    /// Gia hạn đơn thuê
    /// </summary>
    Task<RentalDetailResponse?> Extend(Guid rentalId, ExtendRentalRequest request);

    /// <summary>
    /// Cập nhật đơn thuê
    /// </summary>
    Task<RentalDetailResponse?> Update(Guid rentalId, UpdateRentalRequest request);

    /// <summary>
    /// Tính tổng tiền thuê
    /// </summary>
    Task<decimal> CalculateTotal(Guid carId, DateTime startDate, DateTime endDate, string? couponCode = null);

    /// <summary>
    /// Lấy danh sách đơn thuê của khách hàng
    /// </summary>
    Task<PagedResult<RentalResponse>> GetCustomer(Guid customerId, RentalFilterCriteria criteria);

    /// <summary>
    /// Lấy danh sách đơn thuê của nhà cung cấp
    /// </summary>
    Task<PagedResult<RentalResponse>> GetSupplierRentalsAsync(Guid supplierId, RentalFilterCriteria criteria);

    /// <summary>
    /// Tìm kiếm đơn thuê
    /// </summary>
    Task<PagedResult<RentalResponse>> Search(RentalFilterCriteria criteria);

    /// <summary>
    /// Lấy các đơn thuê đang active
    /// </summary>
    Task<IEnumerable<RentalResponse>> GetActiveRentalsAsync();
}
