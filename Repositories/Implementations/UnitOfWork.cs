using Microsoft.EntityFrameworkCore.Storage;
using RentailCarManagement.Models;
using RentailCarManagement.Repositories.Interfaces;

namespace RentailCarManagement.Repositories.Implementations;

/// <summary>
/// Unit of Work Implementation
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    private ICarRepository? _cars;
    private IRentalRepository? _rentals;
    private IPaymentRepository? _payments;
    private ICustomerRepository? _customers;
    private IReviewRepository? _reviews;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public ICarRepository Cars => _cars ??= new CarRepository(_context);
    public IRentalRepository Rentals => _rentals ??= new RentalRepository(_context);
    public IPaymentRepository Payments => _payments ??= new PaymentRepository(_context);
    public ICustomerRepository Customers => _customers ??= new CustomerRepository(_context);
    public IReviewRepository Reviews => _reviews ??= new ReviewRepository(_context);
    
    public ApplicationDbContext Context => _context;

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
