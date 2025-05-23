using MerchStore.Domain.Entities;

namespace MerchStore.Application.Services.Interfaces;


public interface ICatalogService
{

    Task<IEnumerable<Product>> GetAllProductsAsync(CancellationToken cancellationToken);


    Task<Product?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken);

    // Add search functionality
    Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
    
    // Add write methods
    Task<Product> AddProductAsync(string name, string description, Uri? imageUrl, decimal price, int stockQuantity);
    Task UpdateProductAsync(Guid id, string name, string description, Uri? imageUrl, decimal price, int stockQuantity);
    Task DeleteProductAsync(Guid id);
}