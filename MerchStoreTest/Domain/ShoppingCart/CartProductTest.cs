using System;
using MerchStore.Domain.ShoppingCart;
using MerchStore.Domain.ValueObjects;
using Xunit;
namespace MerchStoreTest.Domain.ShoppingCart;
public class CartProductTest
{
    private static Money ValidMoney() => new Money(100, "SEK");

    [Fact]
    public void Constructor_SetsProperties()
    {
        var product = new CartProduct("prod-1", "Product 1", ValidMoney(), 2);

        Assert.Equal("prod-1", product.ProductId);
        Assert.Equal("Product 1", product.ProductName);
        Assert.Equal(ValidMoney(), product.UnitPrice);
        Assert.Equal(2, product.Quantity);
        Assert.Equal(200, product.TotalPrice.Amount);
        Assert.Equal("SEK", product.TotalPrice.Currency);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Constructor_Throws_WhenProductIdNullOrEmpty(string badId)
    {
        Assert.Throws<ArgumentNullException>(() =>
            new CartProduct(badId, "Product", ValidMoney(), 1));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Constructor_Throws_WhenProductNameNullOrEmpty(string badName)
    {
        Assert.Throws<ArgumentNullException>(() =>
            new CartProduct("prod-1", badName, ValidMoney(), 1));
    }

    [Fact]
    public void Constructor_Throws_WhenUnitPriceNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new CartProduct("prod-1", "Product", null!, 1));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_Throws_WhenQuantityNotPositive(int badQty)
    {
        Assert.Throws<ArgumentException>(() =>
            new CartProduct("prod-1", "Product", ValidMoney(), badQty));
    }

    [Fact]
    public void UpdateQuantity_UpdatesQuantity()
    {
        var product = new CartProduct("prod-1", "Product", ValidMoney(), 2);
        product.UpdateQuantity(5);
        Assert.Equal(5, product.Quantity);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public void UpdateQuantity_Throws_WhenNotPositive(int badQty)
    {
        var product = new CartProduct("prod-1", "Product", ValidMoney(), 2);
        Assert.Throws<ArgumentException>(() => product.UpdateQuantity(badQty));
    }
}