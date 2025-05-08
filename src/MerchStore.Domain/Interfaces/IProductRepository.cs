using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MerchStore.Domain.Entities;

namespace MerchStore.Domain.Interfaces;

public interface IProductRepository : IRepository<Product, Guid>
{
    // Product-specific query methods
    Task<IEnumerable<Product>> GetFeaturedProductsAsync();
    Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category);
    
    // Inventory management
    Task<bool> IsInStockAsync(Guid productId, int quantity);
    Task<bool> UpdateStockAsync(Guid productId, int newQuantity);
    
    // Price management
    Task<IEnumerable<Product>> GetProductsInPriceRangeAsync(decimal minPrice, decimal maxPrice);
}