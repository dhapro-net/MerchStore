using MerchStore.Domain.Entities;
using MerchStore.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MerchStore.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository for querying Order entities.
/// </summary>
public class OrderQueryRepository : IOrderQueryRepository
{
    private readonly AppDbContext _context;

    public OrderQueryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetOrderByIdAsync(Guid orderId)
    {
        return await _context.Orders
            .Include(o => o.Products) // Include related OrderProducts
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }

    public async Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Orders
            .Where(o => o.CreatedDate >= startDate && o.CreatedDate <= endDate)
            .Include(o => o.Products) // Include related OrderProducts
            .ToListAsync();
    }
}