namespace RentailCarManagement.Repositories.Interfaces;

/// <summary>
/// Unit of Work Interface
/// </summary>
public interface IUnitOfWork : IDisposable
{
    ICarRepository Cars { get; }
    IRentalRepository Rentals { get; }
    IPaymentRepository Payments { get; }
    ICustomerRepository Customers { get; }
    IReviewRepository Reviews { get; }

    /// <summary>
    /// Lưu tất cả thay đổi
    /// </summary>
    Task<int> SaveChangesAsync();

    /// <summary>
    /// Bắt đầu transaction
    /// </summary>
    Task BeginTransactionAsync();

    /// <summary>
    /// Commit transaction
    /// </summary>
    Task CommitAsync();

    /// <summary>
    /// Rollback transaction
    /// </summary>
    Task RollbackAsync();
    
    /// <summary>
    /// Get DbContext (for special queries not covered by repositories)
    /// Use sparingly - prefer adding methods to repositories
    /// </summary>
    Models.ApplicationDbContext Context { get; }
}
