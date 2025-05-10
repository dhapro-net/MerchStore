using Microsoft.EntityFrameworkCore;
using MerchStore.Domain.Common;
using MerchStore.Domain.Interfaces;

namespace MerchStore.Infrastructure.Persistence.Repositories;

/// <summary>
/// A generic repository for managing entities (commands).
/// </summary>
/// <typeparam name="TEntity">The entity type this repository works with</typeparam>
/// <typeparam name="TId">The ID type of the entity</typeparam>
public class CommandRepository<TEntity, TId> : ICommandRepository<TEntity, TId>
    where TEntity : Entity<TId>
    where TId : notnull
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public CommandRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public virtual async Task AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public virtual Task UpdateAsync(TEntity entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public virtual Task RemoveAsync(TEntity entity)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }
}