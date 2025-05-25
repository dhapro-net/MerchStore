using MerchStore.Domain.Entities;
using MerchStore.Domain.Interfaces;
using MerchStore.Domain.ValueObjects;

namespace MerchStore.IntegrationTests;

public class FakeProductQueryRepository : IProductQueryRepository
{
    public Task<IEnumerable<Product>> GetAllProductsAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult<IEnumerable<Product>>(new List<Product>());
    }

    public Task<Product?> GetProductByIdAsync(Guid productId, CancellationToken cancellationToken)
    {
        var product = new Product(productId, "Fake Product", "Test Desc", "Test", "image.jpg", new Money(100m, "SEK"), 10);
        return Task.FromResult<Product?>(product);
    }

    public Task<IEnumerable<Product>> GetFeaturedProductsAsync()
    {
        return Task.FromResult<IEnumerable<Product>>(new List<Product>());
    }

    public Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
    {
        return Task.FromResult<IEnumerable<Product>>(new List<Product>());
    }

    public Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category)
    {
        return Task.FromResult<IEnumerable<Product>>(new List<Product>());
    }

    public Task<IEnumerable<Product>> GetProductsInPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        return Task.FromResult<IEnumerable<Product>>(new List<Product>());
    }

    public Task<string?> GetGroupReviewsAsync(Guid productId)
    {
        return Task.FromResult<string?>("Mocked group review data");
    }

    public Task<string?> GetProductNameByIdAsync(Guid productId)
    {
        return Task.FromResult<string?>("Mocked Product Name");
    }
}