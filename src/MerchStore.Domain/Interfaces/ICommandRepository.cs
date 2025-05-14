using MerchStore.Domain.Common;

namespace MerchStore.Domain.Interfaces;

public interface ICommandRepository<TEntity, TId>
    where TEntity : Entity<TId>
    where TId : notnull
{
    Task AddAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task RemoveAsync(TEntity entity);
}