

namespace MerchStore.Domain.Interfaces;

public interface IProductCommandRepository
{
    Task<bool> IsInStockAsync(Guid productId, int quantity);
    Task<bool> UpdateStockAsync(Guid productId, int quantity, CancellationToken cancellationToken);
}