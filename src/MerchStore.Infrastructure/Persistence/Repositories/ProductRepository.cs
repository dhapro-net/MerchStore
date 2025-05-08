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

    public async Task<bool> UpdateStockAsync(Guid productId, int quantity)
    {
        var product = await _context.Set<Product>()
            .FirstOrDefaultAsync(p => p.Id == productId);
        
        if (product == null)
            return false;

        product.StockQuantity = quantity;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Product>> GetProductsInPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        return await _context.Set<Product>()
            .Where(p => p.Price.Amount >= minPrice && p.Price.Amount <= maxPrice)
            .ToListAsync();
    }
}