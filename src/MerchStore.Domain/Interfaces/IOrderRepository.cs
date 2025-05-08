using MerchStore.Domain.Entities;

namespace MerchStore.Domain.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetOrderByIdAsync(Guid orderId);
 
    Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);

    Task<Order> AddOrderAsync(Order order);

    Task UpdateOrderAsync(Order order);

    Task DeleteOrderAsync(Guid orderId);
}