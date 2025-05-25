using MerchStore.Domain.Entities;
using MerchStore.Application.DTOs;

namespace MerchStore.Application.Services.Interfaces;

public interface IOrderService
{
    Task<IEnumerable<Order>> GetAllAsync();
    Task CreateAsync(OrderProductDto model);
}
