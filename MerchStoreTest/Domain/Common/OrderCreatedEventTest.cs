using System;
using MerchStore.Domain.Entities;
using MerchStore.Domain.Events;
using Xunit;
namespace MerchStoreTest.Domain.Common;

public class OrderCreatedEventTest
{
    [Fact]
    public void Constructor_SetsOrderAndOccurredOn()
    {
        var order = new Order(
            Guid.NewGuid(),
            new MerchStore.Domain.ValueObjects.PaymentInfo("1234567812345678", "12/25", "123"),
            "Customer",
            "Address",
            new MerchStore.Domain.ValueObjects.Money(100, "SEK"),
            new System.Collections.Generic.List<OrderProduct>
            {
                new OrderProduct(
    Guid.NewGuid(),
    Guid.NewGuid(),
    "Test Product",
    new MerchStore.Domain.ValueObjects.Money(100, "SEK"),
    1,
    Guid.NewGuid()
)
            },
            DateTime.UtcNow
        );

        var evt = new OrderCreatedEvent(order);

        Assert.Equal(order, evt.Order);
        Assert.True((DateTime.UtcNow - evt.OccurredOn).TotalSeconds < 5);
    }

    [Fact]
    public void Constructor_Throws_WhenOrderIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new OrderCreatedEvent(null!));
    }
}