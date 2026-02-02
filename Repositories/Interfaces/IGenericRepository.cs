using System.Linq.Expressions;

namespace RentailCarManagement.Repositories.Interfaces;

/// <summary>
/// Generic Repository Interface
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public interface IGenericRepository<T> where T : class
{
    /// <summary>
    /// Lấy tất cả entities
    /// </summary>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Lấy entity theo ID
    /// </summary>
    Task<T?> GetByIdAsync(Guid id);

    /// <summary>
    /// Tìm kiếm entities theo điều kiện
    /// </summary>
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Tìm entity đầu tiên theo điều kiện
    /// </summary>
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Lấy entities với include navigation properties
    /// </summary>
    Task<IEnumerable<T>> GetWithIncludeAsync(
        Expression<Func<T, bool>>? predicate = null,
        params Expression<Func<T, object>>[] includes);

    /// <summary>
    /// Lấy entity đầu tiên với include
    /// </summary>
    Task<T?> GetFirstWithIncludeAsync(
        Expression<Func<T, bool>> predicate,
        params Expression<Func<T, object>>[] includes);

    /// <summary>
    /// Thêm entity mới
    /// </summary>
    Task<T> AddAsync(T entity);

    /// <summary>
    /// Thêm nhiều entities
    /// </summary>
    Task AddRangeAsync(IEnumerable<T> entities);

    /// <summary>
    /// Cập nhật entity
    /// </summary>
    void Update(T entity);

    /// <summary>
    /// Cập nhật nhiều entities
    /// </summary>
    void UpdateRange(IEnumerable<T> entities);

    /// <summary>
    /// Xóa entity
    /// </summary>
    void Delete(T entity);

    /// <summary>
    /// Xóa nhiều entities
    /// </summary>
    void DeleteRange(IEnumerable<T> entities);

    /// <summary>
    /// Đếm số lượng entities
    /// </summary>
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);

    /// <summary>
    /// Kiểm tra tồn tại
    /// </summary>
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Lấy IQueryable để custom query
    /// </summary>
    IQueryable<T> Query();
}
