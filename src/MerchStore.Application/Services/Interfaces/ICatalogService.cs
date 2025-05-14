using MerchStore.Domain.Entities;

namespace MerchStore.Application.Services.Interfaces;


public interface ICatalogService
{

    Task<IEnumerable<Product>> GetAllProductsAsync(CancellationToken cancellationToken);


    Task<Product?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken);
}