using MerchStore.Domain.Entities;

namespace MerchStore.Domain.Interfaces;

public interface IOrderQueryRepository
{
    Task<Order?> GetOrderByIdAsync(Guid orderId);
    Task<IEnumerable<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
}