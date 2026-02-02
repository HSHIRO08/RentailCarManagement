using Microsoft.EntityFrameworkCore;
using RentailCarManagement.Models;
using RentailCarManagement.DTOs.Rental;
using RentailCarManagement.Repositories.Interfaces;

namespace RentailCarManagement.Repositories.Implementations;

/// <summary>
/// Rental Repository Implementation
/// </summary>
public class RentalRepository : GenericRepository<Rental>, IRentalRepository
{
    public RentalRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Rental>> GetActiveRentalsAsync()
    {
        return await _dbSet
            .Include(r => r.Car)
            .Include(r => r.Customer)
                .ThenInclude(c => c.User)
            .Where(r => r.Status == "Active" || r.Status == "Confirmed")
            .OrderByDescending(r => r.StartDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Rental>> GetRentalsByCustomerAsync(Guid customerId)
    {
        return await _dbSet
            .Include(r => r.Car)
                .ThenInclude(c => c.CarImages)
            .Include(r => r.Payment)
            .Where(r => r.CustomerId == customerId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Rental>> GetRentalsByCarAsync(Guid carId)
    {
        return await _dbSet
            .Include(r => r.Customer)
                .ThenInclude(c => c.User)
            .Include(r => r.Payment)
            .Where(r => r.CarId == carId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Rental>> GetRentalsBySupplierAsync(Guid supplierId)
    {
        return await _dbSet
            .Include(r => r.Car)
            .Include(r => r.Customer)
                .ThenInclude(c => c.User)
            .Include(r => r.Payment)
            .Where(r => r.Car.SupplierId == supplierId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<(IEnumerable<Rental> Rentals, int TotalCount)> SearchRentalsAsync(RentalFilterCriteria criteria)
    {
        var query = _dbSet
            .Include(r => r.Car)
                .ThenInclude(c => c.CarImages)
            .Include(r => r.Customer)
                .ThenInclude(c => c.User)
            .Include(r => r.Payment)
            .AsQueryable();

        // Apply filters
        if (criteria.CustomerId.HasValue)
            query = query.Where(r => r.CustomerId == criteria.CustomerId.Value);

        if (criteria.CarId.HasValue)
            query = query.Where(r => r.CarId == criteria.CarId.Value);

        if (criteria.SupplierId.HasValue)
            query = query.Where(r => r.Car.SupplierId == criteria.SupplierId.Value);

        if (!string.IsNullOrEmpty(criteria.Status))
            query = query.Where(r => r.Status == criteria.Status);

        if (criteria.FromDate.HasValue)
            query = query.Where(r => r.StartDate >= criteria.FromDate.Value);

        if (criteria.ToDate.HasValue)
            query = query.Where(r => r.EndDate <= criteria.ToDate.Value);

        if (criteria.MinAmount.HasValue)
            query = query.Where(r => r.TotalAmount >= criteria.MinAmount.Value);

        if (criteria.MaxAmount.HasValue)
            query = query.Where(r => r.TotalAmount <= criteria.MaxAmount.Value);

        var totalCount = await query.CountAsync();

        // Sorting
        query = criteria.SortBy?.ToLower() switch
        {
            "startdate" => criteria.SortOrder?.ToLower() == "asc"
                ? query.OrderBy(r => r.StartDate)
                : query.OrderByDescending(r => r.StartDate),
            "enddate" => criteria.SortOrder?.ToLower() == "asc"
                ? query.OrderBy(r => r.EndDate)
                : query.OrderByDescending(r => r.EndDate),
            "amount" => criteria.SortOrder?.ToLower() == "asc"
                ? query.OrderBy(r => r.TotalAmount)
                : query.OrderByDescending(r => r.TotalAmount),
            _ => query.OrderByDescending(r => r.CreatedAt)
        };

        // Pagination
        var rentals = await query
            .Skip((criteria.Page - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToListAsync();

        return (rentals, totalCount);
    }

    public async Task<Rental?> GetRentalWithDetailsAsync(Guid rentalId)
    {
        return await _dbSet
            .Include(r => r.Car)
                .ThenInclude(c => c.Category)
            .Include(r => r.Car)
                .ThenInclude(c => c.CarImages)
            .Include(r => r.Customer)
                .ThenInclude(c => c.User)
            .Include(r => r.Payment)
            .Include(r => r.Review)
            .Include(r => r.Commission)
            .Include(r => r.Complaints)
            .FirstOrDefaultAsync(r => r.RentalId == rentalId);
    }

    public async Task<bool> HasScheduleConflictAsync(Guid carId, DateTime startDate, DateTime endDate, Guid? excludeRentalId = null)
    {
        var query = _dbSet
            .Where(r => r.CarId == carId &&
                        r.Status != "Cancelled" && r.Status != "Completed" &&
                        r.StartDate < endDate && r.EndDate > startDate);

        if (excludeRentalId.HasValue)
            query = query.Where(r => r.RentalId != excludeRentalId.Value);

        return await query.AnyAsync();
    }

    public async Task<IEnumerable<Rental>> GetExpiringRentalsAsync(int daysFromNow = 1)
    {
        var targetDate = DateTime.UtcNow.AddDays(daysFromNow);

        return await _dbSet
            .Include(r => r.Car)
            .Include(r => r.Customer)
                .ThenInclude(c => c.User)
            .Where(r => r.Status == "Active" &&
                        r.EndDate.Date == targetDate.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<Rental>> GetOverdueRentalsAsync()
    {
        return await _dbSet
            .Include(r => r.Car)
            .Include(r => r.Customer)
                .ThenInclude(c => c.User)
            .Where(r => r.Status == "Active" &&
                        r.EndDate < DateTime.UtcNow)
            .ToListAsync();
    }

    public async Task<decimal> GetRevenueAsync(DateTime fromDate, DateTime toDate, Guid? supplierId = null)
    {
        var query = _dbSet
            .Where(r => r.Status == "Completed" &&
                        r.CreatedAt >= fromDate &&
                        r.CreatedAt <= toDate);

        if (supplierId.HasValue)
            query = query.Where(r => r.Car.SupplierId == supplierId.Value);

        return await query.SumAsync(r => r.TotalAmount);
    }
}
