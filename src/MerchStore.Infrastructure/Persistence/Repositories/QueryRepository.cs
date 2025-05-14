using Microsoft.EntityFrameworkCore;
using MerchStore.Domain.Common;
using MerchStore.Domain.Interfaces;

namespace MerchStore.Infrastructure.Persistence.Repositories;

/// <summary>
/// A generic repository for querying entities.
/// </summary>
/// <typeparam name="TEntity">The entity type this repository works with</typeparam>
/// <typeparam name="TId">The ID type of the entity</typeparam>
public class QueryRepository<TEntity, TId> : IQueryRepository<TEntity, TId>
    where TEntity : Entity<TId>
    where TId : notnull
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public QueryRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public virtual async Task<TEntity?> GetByIdAsync(TId id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }
}