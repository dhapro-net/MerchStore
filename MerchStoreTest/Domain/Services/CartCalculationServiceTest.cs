using System.Collections.Generic;
using MerchStore.Domain.Services;
using MerchStore.Domain.ShoppingCart;
using MerchStore.Domain.ValueObjects;
using Xunit;

namespace MerchStoreTest.Domain.Services;
public class CartCalculationServiceTest
{
    [Fact]
    public void CalculateTotal_ReturnsZeroMoney_WhenProductsIsNull()
    {
        var service = new CartCalculationService();

        var result = service.CalculateTotal(null);

        Assert.Equal(0, result.Amount);
        Assert.Equal("SEK", result.Currency);
    }

    [Fact]
    public void CalculateTotal_ReturnsZeroMoney_WhenProductsIsEmpty()
    {
        var service = new CartCalculationService();

        var result = service.CalculateTotal(new List<CartProduct>());

        Assert.Equal(0, result.Amount);
        Assert.Equal("SEK", result.Currency);
    }

    [Fact]
    public void CalculateTotal_ReturnsSumOfProducts()
    {
        var service = new CartCalculationService();
        var products = new List<CartProduct>
        {
            new CartProduct("prod-1", "Product 1", new Money(100, "SEK"), 2),
            new CartProduct("prod-2", "Product 2", new Money(50, "SEK"), 3)
        };

        var result = service.CalculateTotal(products);

        Assert.Equal(100 * 2 + 50 * 3, result.Amount);
        Assert.Equal("SEK", result.Currency);
    }
}