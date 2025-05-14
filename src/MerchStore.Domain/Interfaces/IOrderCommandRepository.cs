using MerchStore.Domain.Entities;

namespace MerchStore.Domain.Interfaces;

public interface IOrderCommandRepository
{
    Task<Order> AddOrderAsync(Order order);
    Task UpdateOrderAsync(Order order);
    Task DeleteOrderAsync(Guid orderId);
}