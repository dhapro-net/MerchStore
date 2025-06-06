using MerchStore.Domain.Entities;

namespace MerchStore.Domain.Interfaces;

public interface IProductQueryRepository
{
    Task<IEnumerable<Product>> GetAllProductsAsync(CancellationToken cancellationToken);
    Task<Product?> GetProductByIdAsync(Guid productId, CancellationToken cancellationToken);
    Task<IEnumerable<Product>> GetFeaturedProductsAsync();
    Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category);
    Task<IEnumerable<Product>> GetProductsInPriceRangeAsync(decimal minPrice, decimal maxPrice);

    //add this for fetch api from JIN
    Task<string?> GetGroupReviewsAsync(Guid productId);
    Task<string?> GetProductNameByIdAsync(Guid productId);

}