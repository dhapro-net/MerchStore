using System;
using MerchStore.Domain.Entities;
using MerchStore.Domain.ValueObjects;
using Xunit;
namespace MerchStoreTest.Domain.Entities;
public class OrderProductTest
{
    private static Money ValidMoney() => new Money(100, "SEK");

    [Fact]
    public void Constructor_SetsProperties()
    {
        var id = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var productName = "Test Product";
        var unitPrice = ValidMoney();
        var quantity = 3;
        var orderId = Guid.NewGuid();

        var op = new OrderProduct(id, productId, productName, unitPrice, quantity, orderId);

        Assert.Equal(id, op.Id);
        Assert.Equal(productId, op.ProductId);
        Assert.Equal(productName, op.ProductName);
        Assert.Equal(unitPrice, op.UnitPrice);
        Assert.Equal(quantity, op.Quantity);
        Assert.Equal(orderId, op.OrderId);
        Assert.Equal(unitPrice.Amount * quantity, op.TotalPrice.Amount);
        Assert.Equal(unitPrice.Currency, op.TotalPrice.Currency);
    }

    [Fact]
    public void Constructor_Throws_WhenProductNameIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new OrderProduct(Guid.NewGuid(), Guid.NewGuid(), null!, ValidMoney(), 1, Guid.NewGuid()));
    }

    [Fact]
    public void Constructor_Throws_WhenUnitPriceIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new OrderProduct(Guid.NewGuid(), Guid.NewGuid(), "Test", null!, 1, Guid.NewGuid()));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_Throws_WhenQuantityIsNotPositive(int badQty)
    {
        Assert.Throws<ArgumentException>(() =>
            new OrderProduct(Guid.NewGuid(), Guid.NewGuid(), "Test", ValidMoney(), badQty, Guid.NewGuid()));
    }

    [Fact]
    public void UpdateQuantity_UpdatesQuantity()
    {
        var op = new OrderProduct(Guid.NewGuid(), Guid.NewGuid(), "Test", ValidMoney(), 2, Guid.NewGuid());
        op.UpdateQuantity(5);
        Assert.Equal(5, op.Quantity);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void UpdateQuantity_Throws_WhenNotPositive(int badQty)
    {
        var op = new OrderProduct(Guid.NewGuid(), Guid.NewGuid(), "Test", ValidMoney(), 2, Guid.NewGuid());
        Assert.Throws<ArgumentException>(() => op.UpdateQuantity(badQty));
    }

    [Fact]
    public void AlternateConstructor_SetsProperties()
    {
        var guid = Guid.NewGuid();
        var v1 = "Test";
        var money = ValidMoney();
        var v2 = 7;

        var op = new OrderProduct(guid, v1, money, v2);

        Assert.Equal(guid, op.Guid);
        Assert.Equal(v1, op.V1);
        Assert.Equal(money, op.Money);
        Assert.Equal(v2, op.V2);
    }
}