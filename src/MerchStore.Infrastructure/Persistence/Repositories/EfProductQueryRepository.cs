using Microsoft.EntityFrameworkCore;
using MerchStore.Domain.Entities;
using MerchStore.Domain.Interfaces;

namespace MerchStore.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository for querying Product entities.
/// </summary>
public class EfProductQueryRepository : IProductQueryRepository
{
    private readonly AppDbContext _context;

    public EfProductQueryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync(CancellationToken cancellationToken)
    {
        return await _context.Products.ToListAsync(cancellationToken);
    }

    public async Task<Product?> GetProductByIdAsync(Guid productId, CancellationToken cancellationToken)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetFeaturedProductsAsync()
    {
        return await _context.Products
            .Where(p => p.IsFeatured)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await _context.Products.ToListAsync();

        return await _context.Products
            .Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm))
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category)
    {
        return await _context.Products
            .Where(p => p.Category == category)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsInPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        return await _context.Products
            .Where(p => p.Price.Amount >= minPrice && p.Price.Amount <= maxPrice)
            .ToListAsync();
    }

    public async Task<string?> GetGroupReviewsAsync(Guid productId)
    {
        return await _context.Products
            .Where(p => p.Id == productId)
            .Select(p => p.Name)
            .FirstOrDefaultAsync();
    }

    public async Task<string?> GetProductNameByIdAsync(Guid productId)
    {
        return await _context.Products
            .Where(p => p.Id == productId)
            .Select(p => p.Name)
            .FirstOrDefaultAsync();
    }
}