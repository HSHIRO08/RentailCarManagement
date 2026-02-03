using Microsoft.EntityFrameworkCore;
using RentailCarManagement.DTOs.Car;
using RentailCarManagement.DTOs.Common;
using RentailCarManagement.Models;
using RentailCarManagement.Repositories.Interfaces;
using RentailCarManagement.Services.Interfaces;

namespace RentailCarManagement.Services.Implementations;

/// <summary>
/// Car Service Implementation
/// </summary>
public class CarService : ICarService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _context;

    public CarService(IUnitOfWork unitOfWork, ApplicationDbContext context)
    {
        _unitOfWork = unitOfWork;
        _context = context;
    }

    public async Task<PagedResult<CarResponse>> SearchAvailableCarsAsync(CarSearchCriteria criteria)
    {
        var (cars, totalCount) = await _unitOfWork.Cars.SearchCarsAsync(criteria);

        var carResponses = cars.Select(MapToCarResponse).ToList();

        return new PagedResult<CarResponse>
        {
            Items = carResponses,
            TotalItems = totalCount,
            PageNumber = criteria.Page,
            PageSize = criteria.PageSize
        };
    }

    public async Task<CarDetailResponse?> GetCarDetailsAsync(Guid carId)
    {
        var car = await _unitOfWork.Cars.GetCarWithDetailsAsync(carId);
        if (car == null)
            return null;

        var averageRating = await _unitOfWork.Cars.GetAverageRatingAsync(carId);
        var reviewCount = await _unitOfWork.Reviews.CountAsync(r => r.Rental.CarId == carId && r.IsApproved == true);
        var rentalCount = car.Rentals?.Count(r => r.Status == "Completed") ?? 0;

        return new CarDetailResponse
        {
            CarId = car.CarId,
            SupplierId = car.SupplierId,
            CategoryId = car.CategoryId,
            LicensePlate = car.LicensePlate,
            Brand = car.Brand,
            Model = car.Model,
            Year = car.Year,
            Seats = car.Seats,
            FuelType = car.FuelType,
            Transmission = car.Transmission,
            PricePerDay = car.PricePerDay,
            PricePerHour = car.PricePerHour,
            Status = car.Status,
            IsApproved = car.IsApproved,
            CreatedAt = car.CreatedAt,
            UpdatedAt = car.UpdatedAt,
            Category = car.Category != null ? new CarCategoryDto
            {
                CategoryId = car.Category.CategoryId,
                Name = car.Category.Name,
                Description = car.Category.Description
            } : null,
            Supplier = car.Supplier != null ? new SupplierDto
            {
                SupplierId = car.Supplier.SupplierId,
                CompanyName = car.Supplier.CompanyName,
                IsVerified = car.Supplier.IsVerified
            } : null,
        };
    }

    public async Task<bool> CheckCarAvailabilityAsync(Guid carId, DateTime startDate, DateTime endDate)
    {
        return await _unitOfWork.Cars.IsCarAvailableAsync(carId, startDate, endDate);
    }

    public async Task<CarResponse> CreateCarAsync(CreateCarRequest request)
    {
        var car = new Car
        {
            CarId = Guid.NewGuid(),
            SupplierId = request.SupplierId,
            CategoryId = request.CategoryId,
            LicensePlate = request.LicensePlate,
            Brand = request.Brand,
            Model = request.Model,
            Year = request.Year,
            Seats = request.Seats,
            FuelType = request.FuelType,
            Transmission = request.Transmission,
            PricePerDay = request.PricePerDay,
            PricePerHour = request.PricePerHour,
            Status = "Available",
            IsApproved = false,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Cars.AddAsync(car);
        await _unitOfWork.SaveChangesAsync();

        return MapToCarResponse(car);
    }

    public async Task<CarResponse?> UpdateCarAsync(Guid carId, UpdateCarRequest request)
    {
        var car = await _unitOfWork.Cars.GetByIdAsync(carId);
        if (car == null)
            return null;

        if (request.CategoryId.HasValue)
            car.CategoryId = request.CategoryId.Value;
        if (!string.IsNullOrEmpty(request.LicensePlate))
            car.LicensePlate = request.LicensePlate;
        if (!string.IsNullOrEmpty(request.Brand))
            car.Brand = request.Brand;
        if (!string.IsNullOrEmpty(request.Model))
            car.Model = request.Model;
        if (request.Year.HasValue)
            car.Year = request.Year.Value;
        if (request.Seats.HasValue)
            car.Seats = request.Seats.Value;
        if (!string.IsNullOrEmpty(request.FuelType))
            car.FuelType = request.FuelType;
        if (!string.IsNullOrEmpty(request.Transmission))
            car.Transmission = request.Transmission;
        if (request.PricePerDay.HasValue)
            car.PricePerDay = request.PricePerDay.Value;
        if (request.PricePerHour.HasValue)
            car.PricePerHour = request.PricePerHour.Value;
        if (!string.IsNullOrEmpty(request.Status))
            car.Status = request.Status;

        car.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Cars.Update(car);
        await _unitOfWork.SaveChangesAsync();

        return MapToCarResponse(car);
    }

    public async Task<bool> DeleteCarAsync(Guid carId)
    {
        var car = await _unitOfWork.Cars.GetByIdAsync(carId);
        if (car == null)
            return false;

        // Check if car has active rentals
        var hasActiveRentals = await _unitOfWork.Rentals.ExistsAsync(
            r => r.CarId == carId && (r.Status == "Active" || r.Status == "Confirmed"));

        if (hasActiveRentals)
            return false;

        car.Status = "Retired";
        car.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Cars.Update(car);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdateCarStatusAsync(Guid carId, string status)
    {
        var car = await _unitOfWork.Cars.GetByIdAsync(carId);
        if (car == null)
            return false;

        car.Status = status;
        car.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Cars.Update(car);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<decimal> CalculateRentalPriceAsync(Guid carId, int days)
    {
        var car = await _unitOfWork.Cars.GetByIdAsync(carId);
        if (car == null)
            return 0;

        return car.PricePerDay * days;
    }

    public async Task<IEnumerable<CarCategoryDto>> GetCategoriesAsync()
    {
        var categories = await _context.CarCategories
            .Where(c => c.IsActive == true)
            .ToListAsync();

        return categories.Select(c => new CarCategoryDto
        {
            CategoryId = c.CategoryId,
            Name = c.Name,
            Description = c.Description
        });
    }

    public async Task<IEnumerable<CarResponse>> GetCarsBySupplierAsync(Guid supplierId)
    {
        var cars = await _unitOfWork.Cars.GetCarsBySupplierAsync(supplierId);
        return cars.Select(MapToCarResponse);
    }

    public async Task<IEnumerable<CarResponse>> GetPopularCarsAsync(int count = 10)
    {
        var cars = await _unitOfWork.Cars.GetPopularCarsAsync(count);
        return cars.Select(MapToCarResponse);
    }

    private CarResponse MapToCarResponse(Car car)
    {
        return new CarResponse
        {
            CarId = car.CarId,
            LicensePlate = car.LicensePlate,
            Brand = car.Brand,
            Model = car.Model,
            Year = car.Year,
            Seats = car.Seats,
            FuelType = car.FuelType,
            Transmission = car.Transmission,
            PricePerDay = car.PricePerDay,
            PricePerHour = car.PricePerHour,
            Status = car.Status,
            IsApproved = car.IsApproved,
            CategoryName = car.Category?.Name,
            SupplierName = car.Supplier?.CompanyName,
            // Fix: CarImages is a string, not a collection. Adjust logic accordingly.
            PrimaryImageUrl = car.CarImages, // or parse if it's a delimited string, e.g. car.CarImages?.Split(';').FirstOrDefault()
            CreatedAt = car.CreatedAt
        };
    }
}
