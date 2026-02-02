using Microsoft.EntityFrameworkCore;
using RentailCarManagement.Models;
using RentailCarManagement.Repositories.Interfaces;

namespace RentailCarManagement.Repositories.Implementations;

/// <summary>
/// Payment Repository Implementation
/// </summary>
public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
{
    public PaymentRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Payment?> GetPaymentByRentalAsync(Guid rentalId)
    {
        return await _dbSet
            .Include(p => p.Rental)
                .ThenInclude(r => r.Car)
            .Include(p => p.Rental)
                .ThenInclude(r => r.Customer)
                    .ThenInclude(c => c.User)
            .FirstOrDefaultAsync(p => p.RentalId == rentalId);
    }

    public async Task<IEnumerable<Payment>> GetPaymentsByCustomerAsync(Guid customerId)
    {
        return await _dbSet
            .Include(p => p.Rental)
                .ThenInclude(r => r.Car)
            .Where(p => p.Rental.CustomerId == customerId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Payment>> GetPaymentsByStatusAsync(string status)
    {
        return await _dbSet
            .Include(p => p.Rental)
            .Where(p => p.Status == status)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalRevenueAsync(DateTime fromDate, DateTime toDate)
    {
        return await _dbSet
            .Where(p => p.Status == "Paid" &&
                        p.PaidAt >= fromDate &&
                        p.PaidAt <= toDate)
            .SumAsync(p => p.Amount);
    }

    public async Task<IEnumerable<Payment>> GetPendingPaymentsAsync()
    {
        return await _dbSet
            .Include(p => p.Rental)
                .ThenInclude(r => r.Car)
            .Include(p => p.Rental)
                .ThenInclude(r => r.Customer)
                    .ThenInclude(c => c.User)
            .Where(p => p.Status == "Pending")
            .OrderBy(p => p.CreatedAt)
            .ToListAsync();
    }
}
