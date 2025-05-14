using Microsoft.EntityFrameworkCore;
using MerchStore.Domain.Entities;
using MerchStore.Domain.Interfaces;

namespace MerchStore.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository for managing Product entities (commands).
/// </summary>
public class ProductCommandRepository : IProductCommandRepository
{
    private readonly AppDbContext _context;

    public ProductCommandRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsInStockAsync(Guid productId, int quantity)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == productId);

        return product != null && product.StockQuantity >= quantity;
    }

    public async Task<bool> UpdateStockAsync(Guid productId, int quantity, CancellationToken cancellationToken)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);

        if (product == null)
        {
            return false;
        }

        product.UpdateStock(quantity);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}