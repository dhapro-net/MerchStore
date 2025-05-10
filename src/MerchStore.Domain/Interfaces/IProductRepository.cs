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

    Task<IEnumerable<Product>> GetAllProductsAsync(CancellationToken cancellationToken);
    Task<Product> GetProductByIdAsync(Guid productId, CancellationToken cancellationToken);


    // Inventory management
    Task<bool> IsInStockAsync(Guid productId, int quantity);
    public interface IProductRepository
    {
        Task<bool> UpdateStockAsync(Guid productId, int quantity, CancellationToken cancellationToken);
    }

    // Price management
    Task<IEnumerable<Product>> GetProductsInPriceRangeAsync(decimal minPrice, decimal maxPrice);
}