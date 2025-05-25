using MerchStore.Application.DTOs;
using MerchStore.Application.Services.Interfaces;
using MerchStore.Domain.Entities;
using MerchStore.Domain.ValueObjects;
using MongoDB.Driver;

namespace MerchStore.Application.Services.Implementations;

public class OrderService : IOrderService
{
    private readonly IMongoCollection<Order> _orders;

    public OrderService(IMongoDatabase database)
    {
        _orders = database.GetCollection<Order>("Orders");
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        return await _orders.Find(_ => true).ToListAsync();
    }

    public async Task CreateAsync(OrderProductDto model)
    {
        var order = new Order(
            Guid.NewGuid(),
            new PaymentInfo("4111111111111111", DateTime.UtcNow.AddYears(1).ToString("MM/yy"), "123"),
            model.CustomerName,
            $"{model.StreetAddress}, {model.City}, {model.ZipCode}, {model.Country}",
            new Money(0, "SEK"), // Placeholder, you can calculate total from OrderProducts later
            new List<OrderProduct>(), // Add products if needed
            DateTime.UtcNow
        );

        await _orders.InsertOneAsync(order);
    }
}