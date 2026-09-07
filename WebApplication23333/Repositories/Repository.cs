using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WebApplication23333.Data;

namespace WebApplication23333.Repositories;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    private readonly AppDbContext _context;
    private readonly DbSet<TEntity> _set;

    public Repository(AppDbContext context)
    {
        _context = context;
        _set = context.Set<TEntity>();
    }

    public async Task<IReadOnlyList<TEntity>> ListAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        bool asNoTracking = true)
    {
        IQueryable<TEntity> query = _set;

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        if (orderBy is not null)
        {
            query = orderBy(query);
        }

        return await query.ToListAsync();
    }

    public async Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        bool asNoTracking = false)
    {
        IQueryable<TEntity> query = _set;

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(predicate);
    }

    public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return _set.AsNoTracking().AnyAsync(predicate);
    }

    public bool Any(Expression<Func<TEntity, bool>> predicate)
    {
        return _set.AsNoTracking().Any(predicate);
    }

    public async Task AddAsync(TEntity entity)
    {
        await _set.AddAsync(entity);
    }

    public void Update(TEntity entity)
    {
        _set.Update(entity);
    }

    public void Remove(TEntity entity)
    {
        _set.Remove(entity);
    }

    public Task<int> SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
}
