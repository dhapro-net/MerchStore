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
    private readonly ICommandRepository<Product, Guid> _productCommandRepository;
   

    public CatalogService(IProductQueryRepository productQueryRepository, ICommandRepository<Product, Guid> productCommandRepository)
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

    public async Task<Product> AddProductAsync(string name, string description, Uri? imageUrl, decimal price, int stockQuantity)
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
        return product;
    }

    public async Task UpdateProductAsync(Guid id, string name, string description, Uri? imageUrl, decimal price, int stockQuantity)
    {
        var product = await _productQueryRepository.GetProductByIdAsync(id, CancellationToken.None);
        if (product == null) throw new Exception("Product not found");

        product.UpdateDetails(name, description, imageUrl?.ToString() ?? "");
        product.UpdatePrice(new Money(price, "SEK"));
        product.UpdateStock(stockQuantity);

        await _productCommandRepository.UpdateAsync(product);
    }

    public async Task DeleteProductAsync(Guid id)
    {
        var product = await _productQueryRepository.GetProductByIdAsync(id, CancellationToken.None);
        if (product == null) throw new Exception("Product not found");
        await _productCommandRepository.RemoveAsync(product);
    }


}