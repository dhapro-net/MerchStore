using Microsoft.EntityFrameworkCore;
using MerchStore.Domain.Entities;
using MerchStore.Domain.Interfaces;

namespace MerchStore.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for managing Product entities.
/// This class inherits from the generic Repository class and adds product-specific functionality.
/// </summary>
public class ProductRepository : Repository<Product, Guid>, IProductRepository
{
    private readonly AppDbContext _context;
    private readonly DbSet<Product> _products;

    public ProductRepository(AppDbContext context) : base(context)
    {
        _context = context;
        _products = context.Set<Product>();
    }

public async Task<IEnumerable<Product>> GetAllProductsAsync(CancellationToken cancellationToken)
{
    return await _products.ToListAsync(cancellationToken);
}

public async Task<Product> GetProductByIdAsync(Guid productId, CancellationToken cancellationToken)
{
    return await _products.FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
}
    public async Task<IEnumerable<Product>> GetFeaturedProductsAsync()
    {
        return await _context.Set<Product>()
            .Where(p => p.IsFeatured)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAllAsync();

        return await _context.Set<Product>()
            .Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm))
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category)
    {
        return await _context.Set<Product>()
            .Where(p => p.Category == category)
            .ToListAsync();
    }

    public async Task<bool> IsInStockAsync(Guid productId, int quantity)
    {
        var product = await _context.Set<Product>()
            .FirstOrDefaultAsync(p => p.Id == productId);

        return product != null && product.StockQuantity >= quantity;
    }

public async Task<bool> UpdateStockAsync(Guid productId, int quantity, CancellationToken cancellationToken)
{
    // Retrieve the product from the database
    var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);

    if (product == null)
    {
        // Product not found
        return false;
    }

    // Update the stock using the domain method
    product.UpdateStock(quantity);

    // Save changes to the database
    await _context.SaveChangesAsync(cancellationToken);

    return true;
}

    public async Task<IEnumerable<Product>> GetProductsInPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        return await _context.Set<Product>()
            .Where(p => p.Price.Amount >= minPrice && p.Price.Amount <= maxPrice)
            .ToListAsync();
    }
}