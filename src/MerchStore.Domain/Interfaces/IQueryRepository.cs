using MerchStore.Domain.Common;

namespace MerchStore.Domain.Interfaces;

public interface IQueryRepository<TEntity, TId>
    where TEntity : Entity<TId>
    where TId : notnull
{
    Task<TEntity?> GetByIdAsync(TId id);
    Task<IEnumerable<TEntity>> GetAllAsync();
}