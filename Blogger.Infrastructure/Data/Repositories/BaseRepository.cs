using System.Linq.Expressions;
using Blogger.Contracts.Data.Repositories;
using Blogger.Migrations;
using Microsoft.EntityFrameworkCore;

namespace Blogger.Infrastructure.Data.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly ApplicationDbContext _context;
    private protected readonly DbSet<T> DbSet;

    protected Repository(ApplicationDbContext context)
    {
        _context = context;
        DbSet = _context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await DbSet.ToListAsync();
    }

    public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate,
        Expression<Func<T, object>> orderBy, bool orderByDescending, int skipCount, int takeCount)
    {
        var query = DbSet.Where(predicate);
        query = orderByDescending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);
        return await query.Skip(skipCount).Take(takeCount).ToListAsync();
    }

    public async Task<T> GetByIdAsync(int id)
    {
        return await DbSet.FindAsync(id);
    }

    public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate)
    {
        var x = await DbSet.FirstOrDefaultAsync(predicate);
        return x;
    }

    public async Task AddAsync(T entity)
    {
        await DbSet.AddAsync(entity);
    }

    public async void UpdateAsync(T entity)
    {
        DbSet.Update(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await DbSet.FindAsync(id);
        if (entity == null) return;
        DbSet.Remove(entity);
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
    {
        return await DbSet.CountAsync(predicate);
    }

    public async Task<int> CountAsync()
    {
        return await DbSet.CountAsync();
    }
}