using System;
using System.Collections.Generic;
using MerchStore.Domain.ShoppingCart;
using MerchStore.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
namespace MerchStoreTest.Domain.ShoppingCart;
public class CartTest
{
    private static Money ValidMoney() => new Money(100, "SEK");

    [Fact]
    public void Constructor_SetsProperties()
    {
        var id = Guid.NewGuid();
        var products = new List<CartProduct>
        {
            new CartProduct("prod-1", "Product 1", ValidMoney(), 2)
        };
        var created = DateTime.UtcNow.AddMinutes(-10);
        var updated = DateTime.UtcNow;

        var cart = new Cart(id, products, created, updated);

        Assert.Equal(id, cart.CartId);
        Assert.Equal(products, cart.Products);
        Assert.Equal(created, cart.CreatedAt);
        Assert.Equal(updated, cart.LastUpdated);
    }

    [Fact]
    public void AddProduct_AddsNewProduct_AndRaisesEvent()
    {
        var cart = new Cart(Guid.NewGuid(), new List<CartProduct>(), DateTime.UtcNow, DateTime.UtcNow);
        var productId = "prod-1";
        var name = "Product 1";
        var price = ValidMoney();
        var quantity = 2;

        cart.AddProduct(productId, name, price, quantity);

        Assert.Single(cart.Products);
        Assert.Equal(productId, cart.Products[0].ProductId);
        Assert.Equal(quantity, cart.Products[0].Quantity);
        Assert.Contains(cart.DomainEvents, e => e.GetType().Name == "CartProductAddedEvent");
    }

    [Fact]
    public void AddProduct_IncrementsQuantity_IfProductExists()
    {
        var cart = new Cart(Guid.NewGuid(), new List<CartProduct>(), DateTime.UtcNow, DateTime.UtcNow);
        cart.AddProduct("prod-1", "Product 1", ValidMoney(), 2);
        cart.AddProduct("prod-1", "Product 1", ValidMoney(), 3);

        Assert.Single(cart.Products);
        Assert.Equal(5, cart.Products[0].Quantity);
    }

    [Fact]
    public void AddProduct_Throws_WhenProductIdNullOrEmpty()
    {
        var cart = new Cart(Guid.NewGuid(), new List<CartProduct>(), DateTime.UtcNow, DateTime.UtcNow);
        Assert.Throws<ArgumentException>(() => cart.AddProduct(null, "Product", ValidMoney(), 1));
        Assert.Throws<ArgumentException>(() => cart.AddProduct("", "Product", ValidMoney(), 1));
    }

    [Fact]
    public void AddProduct_Throws_WhenQuantityNotPositive()
    {
        var cart = new Cart(Guid.NewGuid(), new List<CartProduct>(), DateTime.UtcNow, DateTime.UtcNow);
        Assert.Throws<ArgumentException>(() => cart.AddProduct("prod-1", "Product", ValidMoney(), 0));
        Assert.Throws<ArgumentException>(() => cart.AddProduct("prod-1", "Product", ValidMoney(), -1));
    }

    [Fact]
    public void CalculateTotal_ReturnsZero_WhenNoProducts()
    {
        var cart = new Cart(Guid.NewGuid(), new List<CartProduct>(), DateTime.UtcNow, DateTime.UtcNow);
        var total = cart.CalculateTotal();
        Assert.Equal(0, total.Amount);
        Assert.Equal("SEK", total.Currency);
    }

    [Fact]
    public void CalculateTotal_ReturnsSum_WhenProductsExist()
    {
        var cart = new Cart(Guid.NewGuid(), new List<CartProduct>(), DateTime.UtcNow, DateTime.UtcNow);
        cart.AddProduct("prod-1", "Product 1", new Money(10, "SEK"), 2);
        cart.AddProduct("prod-2", "Product 2", new Money(5, "SEK"), 3);

        var total = cart.CalculateTotal();
        Assert.Equal(10 * 2 + 5 * 3, total.Amount);
        Assert.Equal("SEK", total.Currency);
    }

    [Fact]
    public void Clear_RemovesAllProducts_AndRaisesEvent()
    {
        var cart = new Cart(Guid.NewGuid(), new List<CartProduct>(), DateTime.UtcNow, DateTime.UtcNow);
        cart.AddProduct("prod-1", "Product 1", ValidMoney(), 2);

        cart.Clear();

        Assert.Empty(cart.Products);
        Assert.Contains(cart.DomainEvents, e => e.GetType().Name == "CartClearedEvent");
    }

    [Fact]
    public void RemoveProduct_RemovesProduct_AndRaisesEvent()
    {
        var cart = new Cart(Guid.NewGuid(), new List<CartProduct>(), DateTime.UtcNow, DateTime.UtcNow);
        cart.AddProduct("prod-1", "Product 1", ValidMoney(), 2);

        var removed = cart.RemoveProduct("prod-1");

        Assert.True(removed);
        Assert.Empty(cart.Products);
        Assert.Contains(cart.DomainEvents, e => e.GetType().Name == "CartProductRemovedEvent");
    }

    [Fact]
    public void RemoveProduct_ReturnsFalse_WhenProductNotFound()
    {
        var cart = new Cart(Guid.NewGuid(), new List<CartProduct>(), DateTime.UtcNow, DateTime.UtcNow);
        var removed = cart.RemoveProduct("not-exist");
        Assert.False(removed);
    }

    [Fact]
    public void RemoveProduct_Throws_WhenProductIdNullOrEmpty()
    {
        var cart = new Cart(Guid.NewGuid(), new List<CartProduct>(), DateTime.UtcNow, DateTime.UtcNow);
        Assert.Throws<ArgumentException>(() => cart.RemoveProduct(null));
        Assert.Throws<ArgumentException>(() => cart.RemoveProduct(""));
    }

    [Fact]
    public void UpdateQuantity_UpdatesQuantity_WhenProductExists()
    {
        var cart = new Cart(Guid.NewGuid(), new List<CartProduct>(), DateTime.UtcNow, DateTime.UtcNow);
        cart.AddProduct("prod-1", "Product 1", ValidMoney(), 2);

        var updated = cart.UpdateQuantity("prod-1", 5);

        Assert.True(updated);
        Assert.Equal(5, cart.Products[0].Quantity);
    }

    [Fact]
    public void UpdateQuantity_RemovesProduct_WhenQuantityZeroOrLess()
    {
        var cart = new Cart(Guid.NewGuid(), new List<CartProduct>(), DateTime.UtcNow, DateTime.UtcNow);
        cart.AddProduct("prod-1", "Product 1", ValidMoney(), 2);

        var updated = cart.UpdateQuantity("prod-1", 0);

        Assert.True(updated);
        Assert.Empty(cart.Products);
    }

    [Fact]
    public void UpdateQuantity_ReturnsFalse_WhenProductNotFound()
    {
        var cart = new Cart(Guid.NewGuid(), new List<CartProduct>(), DateTime.UtcNow, DateTime.UtcNow);
        var updated = cart.UpdateQuantity("not-exist", 5);
        Assert.False(updated);
    }

    [Fact]
    public void UpdateQuantity_Throws_WhenProductIdNullOrEmpty()
    {
        var cart = new Cart(Guid.NewGuid(), new List<CartProduct>(), DateTime.UtcNow, DateTime.UtcNow);
        Assert.Throws<ArgumentException>(() => cart.UpdateQuantity(null, 1));
        Assert.Throws<ArgumentException>(() => cart.UpdateQuantity("", 1));
    }

    [Fact]
    public void Create_Throws_WhenCartIdEmpty()
    {
        var logger = new Mock<ILogger<Cart>>().Object;
        Assert.Throws<ArgumentException>(() => Cart.Create(Guid.Empty, logger));
    }

    [Fact]
    public void Create_Throws_WhenLoggerNull()
    {
        Assert.Throws<ArgumentNullException>(() => Cart.Create(Guid.NewGuid(), null));
    }

    [Fact]
    public void Create_ReturnsCart_WithValidArguments()
    {
        var loggerMock = new Mock<ILogger<Cart>>();
        var cartId = Guid.NewGuid();

        var cart = Cart.Create(cartId, loggerMock.Object);

        Assert.Equal(cartId, cart.CartId);
        Assert.Empty(cart.Products);
        Assert.True((DateTime.UtcNow - cart.CreatedAt).TotalSeconds < 5);
        Assert.True((DateTime.UtcNow - cart.LastUpdated).TotalSeconds < 5);
    }
}