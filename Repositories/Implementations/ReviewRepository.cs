using Microsoft.EntityFrameworkCore;
using RentailCarManagement.Models;
using RentailCarManagement.DTOs.Review;
using RentailCarManagement.Repositories.Interfaces;

namespace RentailCarManagement.Repositories.Implementations;

/// <summary>
/// Review Repository Implementation
/// </summary>
public class ReviewRepository : GenericRepository<Review>, IReviewRepository
{
    public ReviewRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Review>> GetReviewsByCarAsync(Guid carId)
    {
        return await _dbSet
            .Include(r => r.Customer)
                .ThenInclude(c => c.User)
            .Include(r => r.Rental)
            .Where(r => r.Rental.CarId == carId && r.IsApproved == true)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Review>> GetReviewsByCustomerAsync(Guid customerId)
    {
        return await _dbSet
            .Include(r => r.Rental)
                .ThenInclude(r => r.Car)
            .Where(r => r.CustomerId == customerId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<Review?> GetReviewByRentalAsync(Guid rentalId)
    {
        return await _dbSet
            .Include(r => r.Customer)
                .ThenInclude(c => c.User)
            .FirstOrDefaultAsync(r => r.RentalId == rentalId);
    }

    public async Task<(IEnumerable<Review> Reviews, int TotalCount)> SearchReviewsAsync(ReviewFilterCriteria criteria)
    {
        var query = _dbSet
            .Include(r => r.Customer)
                .ThenInclude(c => c.User)
            .Include(r => r.Rental)
                .ThenInclude(r => r.Car)
            .AsQueryable();

        // Apply filters
        if (criteria.CarId.HasValue)
            query = query.Where(r => r.Rental.CarId == criteria.CarId.Value);

        if (criteria.CustomerId.HasValue)
            query = query.Where(r => r.CustomerId == criteria.CustomerId.Value);

        if (criteria.SupplierId.HasValue)
            query = query.Where(r => r.Rental.Car.SupplierId == criteria.SupplierId.Value);

        if (criteria.MinRating.HasValue)
            query = query.Where(r => r.Rating >= criteria.MinRating.Value);

        if (criteria.MaxRating.HasValue)
            query = query.Where(r => r.Rating <= criteria.MaxRating.Value);

        if (criteria.IsApproved.HasValue)
            query = query.Where(r => r.IsApproved == criteria.IsApproved.Value);

        if (criteria.FromDate.HasValue)
            query = query.Where(r => r.CreatedAt >= criteria.FromDate.Value);

        if (criteria.ToDate.HasValue)
            query = query.Where(r => r.CreatedAt <= criteria.ToDate.Value);

        var totalCount = await query.CountAsync();

        // Sorting
        query = criteria.SortBy?.ToLower() switch
        {
            "rating" => criteria.SortOrder?.ToLower() == "asc"
                ? query.OrderBy(r => r.Rating)
                : query.OrderByDescending(r => r.Rating),
            "createdat" => criteria.SortOrder?.ToLower() == "asc"
                ? query.OrderBy(r => r.CreatedAt)
                : query.OrderByDescending(r => r.CreatedAt),
            _ => query.OrderByDescending(r => r.CreatedAt)
        };

        // Pagination
        var reviews = await query
            .Skip((criteria.Page - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToListAsync();

        return (reviews, totalCount);
    }

    public async Task<decimal?> GetAverageRatingByCarAsync(Guid carId)
    {
        var reviews = await _dbSet
            .Where(r => r.Rental.CarId == carId && r.IsApproved == true && r.Rating.HasValue)
            .ToListAsync();

        if (!reviews.Any())
            return null;

        return (decimal)reviews.Average(r => r.Rating!.Value);
    }

    public async Task<decimal?> GetAverageRatingBySupplierAsync(Guid supplierId)
    {
        var reviews = await _dbSet
            .Include(r => r.Rental)
                .ThenInclude(r => r.Car)
            .Where(r => r.Rental.Car.SupplierId == supplierId && r.IsApproved == true && r.Rating.HasValue)
            .ToListAsync();

        if (!reviews.Any())
            return null;

        return (decimal)reviews.Average(r => r.Rating!.Value);
    }

    public async Task<bool> HasCustomerReviewedRentalAsync(Guid customerId, Guid rentalId)
    {
        return await _dbSet.AnyAsync(r => r.CustomerId == customerId && r.RentalId == rentalId);
    }
}
