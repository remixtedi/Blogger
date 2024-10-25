using System.Linq.Expressions;

namespace Blogger.Contracts.Data.Repositories;

public interface IRepository<T> where T : class
{
    public Task<IEnumerable<T>> GetAllAsync();

    public Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> orderBy,
        bool orderByDescending = false);

    public Task<T> GetByIdAsync(int id);
    public Task<T?> GetAsync(Expression<Func<T, bool>> predicate);
    public Task AddAsync(T entity);
    public void UpdateAsync(T entity);
    public Task DeleteAsync(int id);
}