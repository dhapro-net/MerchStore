
using MerchStore.Application.Services.Interfaces;
using MerchStore.Domain.Entities;
using MerchStore.Domain.Interfaces;
using MerchStore.Domain.ValueObjects;

namespace MerchStore.Application.Services.Implementations;

/// <summary>
/// Implementation of the catalog service.
/// Acts as a facade over the repository layer.
/// </summary>
public class CatalogService : ICatalogService
{
    private readonly IProductQueryRepository _productQueryRepository;
    private readonly IProductCommandRepository _productCommandRepository;
   

    public CatalogService(IProductQueryRepository productQueryRepository, IProductCommandRepository productCommandRepository)
    {
        _productQueryRepository = productQueryRepository;
        _productCommandRepository = productCommandRepository;
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync(CancellationToken cancellationToken)
    {
        return await _productQueryRepository.GetAllProductsAsync(cancellationToken);
    }

    public async Task<Product?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _productQueryRepository.GetProductByIdAsync(id, cancellationToken);
    }
    // Add search functionality
    // New methods
    public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
    {
        // Use the interface method directly, no casting to concrete type
        return await _productQueryRepository.SearchProductsAsync(searchTerm);
    }

    public async Task AddProductAsync(string name, string description, Uri? imageUrl, decimal price, int stockQuantity)
    {
        var product = new Product(
            Guid.NewGuid(),
            name,
            description,
            category: "general", // Default category for now
            imageUrl?.ToString() ?? "",
            new Money(price, "SEK"),
            stockQuantity
        );

        await _productCommandRepository.AddAsync(product);
    }

    public async Task UpdateProductAsync(Guid id, string name, string description, Uri? imageUrl, decimal price, int stockQuantity)
{
    var product = await _productQueryRepository.GetProductByIdAsync(id, CancellationToken.None);
    if (product == null) throw new Exception("Product not found");

    // Assuming you have an Update() method in the Product entity
    product.Update(name, description, product.Category, imageUrl?.ToString() ?? "", new Money(price, "SEK"), stockQuantity);

    await _productCommandRepository.UpdateAsync(product);
}

public async Task DeleteProductAsync(Guid id)
{
    await _productCommandRepository.DeleteAsync(id);
}


}