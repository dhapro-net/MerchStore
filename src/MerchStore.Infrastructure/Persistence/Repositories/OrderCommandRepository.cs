using MerchStore.Domain.Entities;
using MerchStore.Domain.Interfaces;

namespace MerchStore.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository for managing Order entities (commands).
/// </summary>
public class OrderCommandRepository : IOrderCommandRepository
{
    private readonly AppDbContext _context;

    public OrderCommandRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Order> AddOrderAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();
        return order;
    }

    public async Task UpdateOrderAsync(Order order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteOrderAsync(Guid orderId)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order != null)
        {
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }
    }
}