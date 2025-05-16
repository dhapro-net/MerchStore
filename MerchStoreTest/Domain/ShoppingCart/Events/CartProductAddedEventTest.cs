using System;
using MerchStore.Domain.ShoppingCart.Events;
using MerchStore.Domain.ValueObjects;
using Xunit;
namespace MerchStoreTest.Domain.ShoppingCart.Events;
public class CartProductAddedEventTest
{
    [Fact]
    public void Constructor_SetsProperties()
    {
        var cartId = Guid.NewGuid();
        var productId = "prod-1";
        var productName = "Test Product";
        var unitPrice = new Money(100, "SEK");
        var quantity = 2;

        var evt = new CartProductAddedEvent(cartId, productId, productName, unitPrice, quantity);

        Assert.Equal(cartId, evt.CartId);
        Assert.Equal(productId, evt.ProductId);
        Assert.Equal(productName, evt.ProductName);
        Assert.Equal(unitPrice, evt.UnitPrice);
        Assert.Equal(quantity, evt.Quantity);
    }

    [Fact]
    public void Constructor_Throws_WhenCartIdIsEmpty()
    {
        Assert.Throws<ArgumentException>(() =>
            new CartProductAddedEvent(Guid.Empty, "prod-1", "Test Product", new Money(100, "SEK"), 1));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_Throws_WhenProductIdIsNullOrWhitespace(string badId)
    {
        Assert.Throws<ArgumentException>(() =>
            new CartProductAddedEvent(Guid.NewGuid(), badId, "Test Product", new Money(100, "SEK"), 1));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_Throws_WhenProductNameIsNullOrWhitespace(string badName)
    {
        Assert.Throws<ArgumentException>(() =>
            new CartProductAddedEvent(Guid.NewGuid(), "prod-1", badName, new Money(100, "SEK"), 1));
    }

    [Fact]
    public void Constructor_Throws_WhenUnitPriceIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new CartProductAddedEvent(Guid.NewGuid(), "prod-1", "Test Product", null!, 1));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_Throws_WhenQuantityIsNotPositive(int badQty)
    {
        Assert.Throws<ArgumentException>(() =>
            new CartProductAddedEvent(Guid.NewGuid(), "prod-1", "Test Product", new Money(100, "SEK"), badQty));
    }
}