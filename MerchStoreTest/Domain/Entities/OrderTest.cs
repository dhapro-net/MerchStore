using System;
using System.Collections.Generic;
using System.Linq;
using MerchStore.Domain.Entities;
using MerchStore.Domain.Events;
using MerchStore.Domain.ValueObjects;
using Xunit;
namespace MerchStoreTest.Domain.Entities;

public class OrderTest
{
    private static PaymentInfo TestPaymentInfo() => new PaymentInfo("1234567812345678", "12/25", "123");
    private static Money TestMoney(decimal amount = 100) => new Money(amount, "SEK");
    private static OrderProduct TestOrderProduct(decimal price = 50, int qty = 2)
        => new OrderProduct(
            Guid.NewGuid(), // id
            Guid.NewGuid(), // orderId
            "prod",         // productName or productCode
            new Money(price, "SEK"), // unitPrice
            qty,            // quantity
            Guid.NewGuid()  // productId or another required Guid
        );

    [Fact]
    public void Constructor_SetsProperties_AndAddsOrderCreatedEvent()
    {
        var id = Guid.NewGuid();
        var paymentInfo = TestPaymentInfo();
        var customerName = "John Doe";
        var address = "123 Main St";
        var totalPrice = TestMoney(100);
        var products = new List<OrderProduct> { TestOrderProduct() };
        var createdDate = DateTime.UtcNow;

        var order = new Order(id, paymentInfo, customerName, address, totalPrice, products, createdDate);

        Assert.Equal(id, order.Id);
        Assert.Equal(paymentInfo, order.PaymentInfo);
        Assert.Equal(customerName, order.CustomerName);
        Assert.Equal(address, order.Address);
        Assert.Equal(totalPrice, order.TotalPrice);
        Assert.Equal(products, order.Products);
        Assert.Equal(createdDate, order.CreatedDate);

        Assert.Single(order.DomainEvents);
        Assert.IsType<OrderCreatedEvent>(order.DomainEvents.First());
        Assert.Equal(order, ((OrderCreatedEvent)order.DomainEvents.First()).Order);
    }

    [Fact]
    public void Constructor_Throws_WhenProductsNullOrEmpty()
    {
        var id = Guid.NewGuid();
        var paymentInfo = TestPaymentInfo();
        var customerName = "John Doe";
        var address = "123 Main St";
        var totalPrice = TestMoney(100);
        var createdDate = DateTime.UtcNow;

        Assert.Throws<ArgumentException>(() =>
            new Order(id, paymentInfo, customerName, address, totalPrice, null!, createdDate));

        Assert.Throws<ArgumentException>(() =>
            new Order(id, paymentInfo, customerName, address, totalPrice, new List<OrderProduct>(), createdDate));
    }

    [Fact]
    public void Constructor_Throws_WhenAnyRequiredArgumentIsNull()
    {
        var id = Guid.NewGuid();
        var products = new List<OrderProduct> { TestOrderProduct() };
        var createdDate = DateTime.UtcNow;

        Assert.Throws<ArgumentNullException>(() =>
            new Order(id, null!, "John Doe", "123 Main St", TestMoney(), products, createdDate));
        Assert.Throws<ArgumentNullException>(() =>
            new Order(id, TestPaymentInfo(), null!, "123 Main St", TestMoney(), products, createdDate));
        Assert.Throws<ArgumentNullException>(() =>
            new Order(id, TestPaymentInfo(), "John Doe", null!, TestMoney(), products, createdDate));
        Assert.Throws<ArgumentNullException>(() =>
            new Order(id, TestPaymentInfo(), "John Doe", "123 Main St", null!, products, createdDate));
    }

    [Fact]
    public void AddDomainEvent_AddsEvent()
    {
        var order = CreateValidOrder();
        var evt = new OrderCreatedEvent(order);

        order.AddDomainEvent(evt);

        Assert.Contains(evt, order.DomainEvents);
    }

    [Fact]
    public void RemoveDomainEvent_RemovesEvent()
    {
        var order = CreateValidOrder();
        var evt = new OrderCreatedEvent(order);

        order.AddDomainEvent(evt);
        order.RemoveDomainEvent(evt);

        Assert.DoesNotContain(evt, order.DomainEvents);
    }

    [Fact]
    public void AddDomainEvent_Throws_WhenNull()
    {
        var order = CreateValidOrder();
        Assert.Throws<ArgumentNullException>(() => order.AddDomainEvent(null!));
    }

    [Fact]
    public void RemoveDomainEvent_Throws_WhenNull()
    {
        var order = CreateValidOrder();
        Assert.Throws<ArgumentNullException>(() => order.RemoveDomainEvent(null!));
    }

    [Fact]
    public void ClearDomainEvents_RemovesAllEvents()
    {
        var order = CreateValidOrder();
        var evt = new OrderCreatedEvent(order);
        order.AddDomainEvent(evt);

        order.ClearDomainEvents();

        Assert.Empty(order.DomainEvents);
    }

    [Fact]
    public void AddProduct_AddsProductAndUpdatesTotalPrice()
    {
        var order = CreateValidOrder();
        var newProduct = TestOrderProduct(25, 4);

        var oldTotal = order.TotalPrice.Amount;
        order.AddProduct(newProduct);

        Assert.Contains(newProduct, order.Products);
        Assert.Equal(order.Products.Sum(p => p.UnitPrice.Amount * p.Quantity), order.TotalPrice.Amount);
        Assert.True(order.TotalPrice.Amount > oldTotal);
    }

    [Fact]
    public void AddProduct_Throws_WhenNull()
    {
        var order = CreateValidOrder();
        Assert.Throws<ArgumentNullException>(() => order.AddProduct(null!));
    }

    private static Order CreateValidOrder()
    {
        return new Order(
            Guid.NewGuid(),
            TestPaymentInfo(),
            "John Doe",
            "123 Main St",
            TestMoney(100),
            new List<OrderProduct> { TestOrderProduct() },
            DateTime.UtcNow
        );
    }
}