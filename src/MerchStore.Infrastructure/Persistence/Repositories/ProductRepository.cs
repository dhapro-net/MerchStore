using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MerchStore.Domain.Entities;
using MerchStore.Domain.Interfaces;
using MerchStore.Domain.ValueObjects;

namespace MerchStore.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for managing Product entities.
/// This class inherits from the generic Repository class and adds product-specific functionality.
/// </summary>
public class ProductRepository : Repository<Product, Guid>, IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetFeaturedProductsAsync()
    {
        // Logic to retrieve featured products (example implementation)
        // This could use a Featured flag on your Product entity or other criteria
        return await _context.Products
            .Where(p => p.IsAvailable && p.StockQuantity > 0)
            .OrderByDescending(p => p.Price.Amount) // Order by price instead of CreatedAt
            .Take(10)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return new List<Product>();

        return await _context.Products
            .Where(p => p.IsAvailable && 
                       (p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm)))
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
            return new List<Product>();

        // Assuming you have a Category property or relationship
        // If you have a separate Categories table, you'd need to join
        return await _context.Products
            .Where(p => p.IsAvailable && p.Category == category)
            .ToListAsync();
    }

    public async Task<bool> IsInStockAsync(Guid productId, int quantity)
    {
        var product = await _context.Products
            .Where(p => p.Id == productId)
            .Select(p => new { p.StockQuantity })
            .FirstOrDefaultAsync();

        return product != null && product.StockQuantity >= quantity;
    }

    public async Task<bool> UpdateStockAsync(Guid productId, int newQuantity)
    {
        if (newQuantity < 0)
            return false;

        var product = await _context.Products.FindAsync(productId);
        if (product == null)
            return false;

        product.UpdateStock(newQuantity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Product>> GetProductsInPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        if (minPrice < 0 || maxPrice < minPrice)
            return new List<Product>();

        return await _context.Products
            .Where(p => p.IsAvailable && 
                       p.Price.Amount >= minPrice && 
                       p.Price.Amount <= maxPrice)
            .OrderBy(p => p.Price.Amount)
            .ToListAsync();
    }
}