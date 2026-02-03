using Microsoft.EntityFrameworkCore;
using RentailCarManagement.Models;
using RentailCarManagement.Repositories.Interfaces;

namespace RentailCarManagement.Repositories.Implementations;

/// <summary>
/// Customer Repository Implementation
/// </summary>
public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Customer?> GetCustomerByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.UserId == userId);
    }

    public async Task<Customer?> GetCustomerWithRentalsAsync(Guid customerId)
    {
        return await _dbSet
            .Include(c => c.User)
            .Include(c => c.Rentals)
                .ThenInclude(r => r.Car)
                    .ThenInclude(navigationPropertyPath: c => c.CarImages)
            .Include(c => c.Rentals)
                .ThenInclude(r => r.Payment)
            .Include(c => c.Reviews)
            .FirstOrDefaultAsync(c => c.CustomerId == customerId);
    }

    public async Task<(int TotalRentals, decimal TotalSpent)> GetCustomerStatsAsync(Guid customerId)
    {
        var rentals = await _context.Rentals
            .Where(r => r.CustomerId == customerId && r.Status == "Completed")
            .ToListAsync();

        return (rentals.Count, rentals.Sum(r => r.TotalAmount));
    }

    public async Task<bool> IsLicenseValidAsync(Guid customerId, DateTime rentalDate)
    {
        var customer = await _dbSet.FindAsync(customerId);
        if (customer == null || !customer.LicenseExpiryDate.HasValue)
            return false;

        return customer.LicenseExpiryDate.Value.ToDateTime(TimeOnly.MinValue) > rentalDate;
    }
}
