using RentailCarManagement.DTOs.Common;
using RentailCarManagement.DTOs.Rental;
using RentailCarManagement.Exceptions;
using RentailCarManagement.Models;
using RentailCarManagement.Repositories.Implementations;
using RentailCarManagement.Repositories.Interfaces;
using RentailCarManagement.Services.Interfaces;
using System.Net.WebSockets;

namespace RentailCarManagement.Services.Implementations;

/// <summary>
/// Rental Service Implementation
/// </summary>
public class RentalService : IRentalService
{
    private readonly IUnitOfWork _unitOfWork;

    public RentalService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<RentalDetailResponse> Create(CreateRentalRequest request)
    {
            if (request.StartDate >= request.EndDate)
                throw new BusinessException("Ngày kết thúc phải sau ngày bắt đầu");

            if (request.StartDate < DateTime.UtcNow.Date)
                throw new BusinessException("Ngày bắt đầu không thể trong quá khứ");

            var isAvailable = await _unitOfWork.Cars.IsCarAvailableAsync(
                request.CarId, request.StartDate, request.EndDate);

            if (!isAvailable)
                throw new BusinessException("Xe không khả dụng trong khoảng thời gian này");

            var car = await _unitOfWork.Cars.GetByIdAsync(request.CarId)
                      ?? throw new NotFoundException("Xe không tồn tại");

            var days = (request.EndDate - request.StartDate).Days;
            if (days < 1) days = 1;
            var totalAmount = car.PricePerDay * days;

            var rental = new Rental
            {
                RentalId = Guid.NewGuid(),
                CarId = request.CarId,
                CustomerId = request.CustomerId,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                TotalAmount = totalAmount,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            var createdRental = await _unitOfWork.Rentals.AddAsync(rental);
            return await Get(createdRental.RentalId)
                ?? throw new Exception("Đã xảy ra lỗi khi tạo đơn thuê");

    }
      


    public async Task<RentalDetailResponse?> Get(Guid rentalId)
    {
        var rental = await _unitOfWork.Rentals.GetRentalWithDetailsAsync(rentalId);
        if (rental == null)
            return null;

        return MapToRentalDetailResponse(rental);
    }

    public async Task<bool> ConfirmRentalAsync(Guid rentalId)
    {
        var rental = await _unitOfWork.Rentals.GetByIdAsync(rentalId);
        if (rental == null || rental.Status != "Pending")
            return false;

        rental.Status = "Confirmed";
        rental.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Rentals.Update(rental);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> Start(Guid rentalId)
    {
            var rental = await _unitOfWork.Rentals.GetByIdAsync(rentalId);
            if (rental == null || rental.Status != "Confirmed")
            {
                await _unitOfWork.RollbackAsync();
                return false;
            }

            rental.Status = "Active";
            rental.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Rentals.Update(rental);

            var car = await _unitOfWork.Cars.GetByIdAsync(rental.CarId);
            if (car != null)
            {
                car.Status = "Rented";
                car.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Cars.Update(car);
            }

            await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> Complete(Guid rentalId, DateTime? actualReturnDate = null)
    {
            var rental = await _unitOfWork.Rentals.GetByIdAsync(rentalId);
            if (rental == null || rental.Status != "Active")
            {
                await _unitOfWork.RollbackAsync();
                return false;
            }

            rental.Status = "Completed";
            rental.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Rentals.Update(rental);

            // Update car status
            var car = await _unitOfWork.Cars.GetByIdAsync(rental.CarId);
            if (car != null)
            {
                car.Status = "Available";
                car.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Cars.Update(car);
            }


            return true;
    }

    public async Task<bool> Cancel(Guid rentalId, string reason)
    {

            var rental = await _unitOfWork.Rentals.GetByIdAsync(rentalId);
            if (rental == null || rental.Status == "Completed" || rental.Status == "Cancelled")
            {
                await _unitOfWork.RollbackAsync();
                return false;
            }

            var wasActive = rental.Status == "Active";  

            rental.Status = "Cancelled";
            rental.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Rentals.Update(rental);

            if (wasActive)  
            {
                var car = await _unitOfWork.Cars.GetByIdAsync(rental.CarId);
                if (car != null)
                {
                    car.Status = "Available";
                    car.UpdatedAt = DateTime.UtcNow;
                    _unitOfWork.Cars.Update(car);
                }
            }

            return true;
       }
        

    public async Task<RentalDetailResponse?> Extend(Guid rentalId, ExtendRentalRequest request)
    {
        var rental = await _unitOfWork.Rentals.GetRentalWithDetailsAsync(rentalId);
        if (rental == null || rental.Status != "Active")
            return null;

        var newEndDate = rental.EndDate.AddDays(request.ExtendedDays);

        var isAvailable = await _unitOfWork.Cars.IsCarAvailableAsync(
            rental.CarId, rental.EndDate, newEndDate, rentalId);

        if (!isAvailable)
            throw new BusinessException("Xe không khả dụng trong khoảng thời gian gia hạn");

        var additionalAmount = rental.Car.PricePerDay * request.ExtendedDays;

        rental.EndDate = newEndDate;
        rental.TotalAmount += additionalAmount;
        rental.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Rentals.Update(rental);

        return await Get(rentalId);
    }

    public async Task<RentalDetailResponse?> Update(Guid rentalId, UpdateRentalRequest request)
    {
        var rental = await _unitOfWork.Rentals.GetByIdAsync(rentalId);
        if (rental == null)
            return null;

        if (request.StartDate.HasValue)
            rental.StartDate = request.StartDate.Value;
        if (request.EndDate.HasValue)
            rental.EndDate = request.EndDate.Value;

        rental.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Rentals.Update(rental);

        return await Get(rentalId);
    }

    public async Task<decimal> CalculateTotal(Guid carId, DateTime startDate, DateTime endDate, string? couponCode = null)
    {
        var car = await _unitOfWork.Cars.GetByIdAsync(carId);
        if (car == null)
            return 0;

        var days = (endDate - startDate).Days;
        if (days < 1) days = 1;

        var total = car.PricePerDay * days;

        return total;
    }

    public async Task<PagedResult<RentalResponse>> GetCustomer(Guid customerId, RentalFilterCriteria criteria)
    {
        criteria.CustomerId = customerId;
        return await Search(criteria);
    }

    public async Task<PagedResult<RentalResponse>> GetSupplierRentalsAsync(Guid supplierId, RentalFilterCriteria criteria)
    {
        criteria.SupplierId = supplierId;
        return await Search(criteria);
    }

    public async Task<PagedResult<RentalResponse>> Search(RentalFilterCriteria criteria)
    {
        var (rentals, totalCount) = await _unitOfWork.Rentals.SearchRentalsAsync(criteria);

        var rentalResponses = rentals.Select(MapToRentalResponse).ToList();

        return new PagedResult<RentalResponse>
        {
            Items = rentalResponses,
            TotalItems = totalCount,
            PageNumber = criteria.Page,
            PageSize = criteria.PageSize
        };
    }

    public async Task<IEnumerable<RentalResponse>> GetActiveRentalsAsync()
    {
        var rentals = await _unitOfWork.Rentals.GetActiveRentalsAsync();
        return rentals.Select(MapToRentalResponse);
    }

    private RentalResponse MapToRentalResponse(Rental rental)
    {
        return new RentalResponse
        {
            RentalId = rental.RentalId,
            CarId = rental.CarId,
            CustomerId = rental.CustomerId,
            StartDate = rental.StartDate,
            EndDate = rental.EndDate,
            TotalAmount = rental.TotalAmount,
            Status = rental.Status,
            CreatedAt = rental.CreatedAt,
            CarBrand = rental.Car?.Brand,
            CarModel = rental.Car?.Model,
            CarLicensePlate = rental.Car?.LicensePlate,
            CarImageUrl = rental.CarImages,
            CustomerName = rental.Customer?.User?.FullName,
            CustomerEmail = rental.Customer?.User?.Email,
            TotalDays = (rental.EndDate - rental.StartDate).Days,
            PaymentStatus = rental.Payment?.Status
        };
    }

    private RentalDetailResponse MapToRentalDetailResponse(Rental rental)
    {
        return new RentalDetailResponse
        {
            RentalId = rental.RentalId,
            StartDate = rental.StartDate,
            EndDate = rental.EndDate,
            TotalAmount = rental.TotalAmount,
            Status = rental.Status,
            CreatedAt = rental.CreatedAt,
            UpdatedAt = rental.UpdatedAt,
            Car = rental.Car != null ? new RentalCarDto
            {
                CarId = rental.Car.CarId,
                Brand = rental.Car.Brand,
                Model = rental.Car.Model,
                LicensePlate = rental.Car.LicensePlate,
                Year = rental.Car.Year,
                FuelType = rental.Car.FuelType,
                Transmission = rental.Car.Transmission,
                PricePerDay = rental.Car.PricePerDay,
                ImageUrl = rental.Car.CarImages
            } : null,
            Customer = rental.Customer != null ? new RentalCustomerDto
            {
                CustomerId = rental.Customer.CustomerId,
                FullName = rental.Customer.User?.FullName,
                Email = rental.Customer.User?.Email,
                Phone = rental.Customer.User?.PhoneNumber,
                DriverLicenseNumber = rental.Customer.DriverLicenseNumber
            } : null,
            Payment = rental.Payment != null ? new RentalPaymentDto
            {
                PaymentId = rental.Payment.PaymentId,
                Amount = rental.Payment.Amount,
                PaymentMethod = rental.Payment.PaymentMethod,
                Status = rental.Payment.Status,
                PaidAt = rental.Payment.PaidAt
            } : null,
            Review = rental.Review != null ? new RentalReviewDto
            {
                ReviewId = rental.Review.ReviewId,
                Rating = rental.Review.Rating,
                Comment = rental.Review.Comment,
                CreatedAt = rental.Review.CreatedAt
            } : null,
            TotalDays = (rental.EndDate - rental.StartDate).Days
        };
    }
}
