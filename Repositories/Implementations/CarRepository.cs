using Microsoft.EntityFrameworkCore;
using RentailCarManagement.Models;
using RentailCarManagement.DTOs.Car;
using RentailCarManagement.Repositories.Interfaces;

namespace RentailCarManagement.Repositories.Implementations;

/// <summary>
/// Car Repository Implementation
/// </summary>
public class CarRepository : GenericRepository<Car>, ICarRepository
{
    public CarRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Car>> GetAvailableCarsAsync(DateTime startDate, DateTime endDate)
    {
        var rentedCarIds = await _context.Rentals
            .Where(r => r.Status != "Cancelled" && r.Status != "Completed" &&
                        r.StartDate < endDate && r.EndDate > startDate)
            .Select(r => r.CarId)
            .Distinct()
            .ToListAsync();

        return await _dbSet
            .Include(c => c.Category)
            .Include(c => c.CarImages)
            .Where(c => c.Status == "Available" &&
                        c.IsApproved == true &&
                        !rentedCarIds.Contains(c.CarId))
            .ToListAsync();
    }

    public async Task<(IEnumerable<Car> Cars, int TotalCount)> SearchCarsAsync(CarSearchCriteria criteria)
    {
        var query = _dbSet
            .Include(c => c.Category)
            .Include(c => c.Supplier)
            .Include(c => c.CarImages)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrEmpty(criteria.Keyword))
        {
            query = query.Where(c =>
                c.Brand.Contains(criteria.Keyword) ||
                c.Model.Contains(criteria.Keyword) ||
                c.LicensePlate.Contains(criteria.Keyword));
        }
        if (criteria.CategoryId.HasValue)
            query = query.Where(c => c.CategoryId == criteria.CategoryId.Value);

        if (criteria.MinPrice.HasValue)
            query = query.Where(c => c.PricePerDay >= criteria.MinPrice.Value);

        if (criteria.MaxPrice.HasValue)
            query = query.Where(c => c.PricePerDay <= criteria.MaxPrice.Value);

        if (!string.IsNullOrEmpty(criteria.FuelType))
            query = query.Where(c => c.FuelType == criteria.FuelType);

        if (!string.IsNullOrEmpty(criteria.Transmission))
            query = query.Where(c => c.Transmission == criteria.Transmission);

        if (criteria.MinSeats.HasValue)
            query = query.Where(c => c.Seats >= criteria.MinSeats.Value);

        if (criteria.MaxSeats.HasValue)
            query = query.Where(c => c.Seats <= criteria.MaxSeats.Value);

        if (criteria.YearFrom.HasValue)
            query = query.Where(c => c.Year >= criteria.YearFrom.Value);

        if (criteria.YearTo.HasValue)
            query = query.Where(c => c.Year <= criteria.YearTo.Value);

        if (criteria.SupplierId.HasValue)
            query = query.Where(c => c.SupplierId == criteria.SupplierId.Value);

        if (criteria.IsApproved.HasValue)
            query = query.Where(c => c.IsApproved == criteria.IsApproved.Value);

        if (!string.IsNullOrEmpty(criteria.Status))
            query = query.Where(c => c.Status == criteria.Status);

        // Check availability
        if (criteria.StartDate.HasValue && criteria.EndDate.HasValue)
        {
            var rentedCarIds = await _context.Rentals
                .Where(r => r.Status != "Cancelled" && r.Status != "Completed" &&
                            r.StartDate < criteria.EndDate.Value && r.EndDate > criteria.StartDate.Value)
                .Select(r => r.CarId)
                .Distinct()
                .ToListAsync();

            query = query.Where(c => !rentedCarIds.Contains(c.CarId));
        }

        var totalCount = await query.CountAsync();

        // Sorting
        query = criteria.SortBy?.ToLower() switch
        {
            "price" => criteria.SortOrder?.ToLower() == "desc"
                ? query.OrderByDescending(c => c.PricePerDay)
                : query.OrderBy(c => c.PricePerDay),
            "year" => criteria.SortOrder?.ToLower() == "desc"
                ? query.OrderByDescending(c => c.Year)
                : query.OrderBy(c => c.Year),
            "createdat" => criteria.SortOrder?.ToLower() == "desc"
                ? query.OrderByDescending(c => c.CreatedAt)
                : query.OrderBy(c => c.CreatedAt),
            _ => query.OrderByDescending(c => c.CreatedAt)
        };

        // Pagination
        var cars = await query
            .Skip((criteria.Page - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToListAsync();

        return (cars, totalCount);
    }

    public async Task<IEnumerable<Car>> GetCarsByCategoryAsync(Guid categoryId)
    {
        return await _dbSet
            .Include(c => c.CarImages)
            .Where(c => c.CategoryId == categoryId && c.IsApproved == true)
            .ToListAsync();
    }

    public async Task<IEnumerable<Car>> GetCarsBySupplierAsync(Guid supplierId)
    {
        return await _dbSet
            .Include(c => c.Category)
            .Include(c => c.CarImages)
            .Where(c => c.SupplierId == supplierId)
            .ToListAsync();
    }

    public async Task<bool> IsCarAvailableAsync(Guid carId, DateTime startDate, DateTime endDate, Guid? excludeRentalId = null)
    {
        var query = _context.Rentals
            .Where(r => r.CarId == carId &&
                        r.Status != "Cancelled" && r.Status != "Completed" &&
                        r.StartDate < endDate && r.EndDate > startDate);

        if (excludeRentalId.HasValue)
            query = query.Where(r => r.RentalId != excludeRentalId.Value);

        return !await query.AnyAsync();
    }

    public async Task<Car?> GetCarWithDetailsAsync(Guid carId)
    {
        return await _dbSet
            .Include(c => c.Category)
            .Include(c => c.Supplier)
                .ThenInclude(s => s.User)
            .Include(c => c.CarImages)
            .Include(c => c.Rentals)
            .FirstOrDefaultAsync(c => c.CarId == carId);
    }

    public async Task<IEnumerable<Car>> GetPopularCarsAsync(int count = 10)
    {
        return await _dbSet
            .Include(c => c.Category)
            .Include(c => c.CarImages)
            .Include(c => c.Rentals)
            .Where(c => c.Status == "Available" && c.IsApproved == true)
            .OrderByDescending(c => c.Rentals.Count)
            .Take(count)
            .ToListAsync();
    }

    public async Task<decimal?> GetAverageRatingAsync(Guid carId)
    {
        var reviews = await _context.Reviews
            .Where(r => r.Rental.CarId == carId && r.IsApproved == true)
            .ToListAsync();

        if (!reviews.Any())
            return null;

        return (decimal)reviews.Average(r => r.Rating ?? 0);
    }
}
