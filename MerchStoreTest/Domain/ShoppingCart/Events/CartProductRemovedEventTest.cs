using System;
using MerchStore.Domain.ShoppingCart.Events;
using Xunit;
namespace MerchStoreTest.Domain.ShoppingCart.Events;
public class CartProductRemovedEventTest
{
    [Fact]
    public void Constructor_SetsProperties()
    {
        var cartId = Guid.NewGuid();
        var productId = "prod-123";

        var evt = new CartProductRemovedEvent(cartId, productId);

        Assert.Equal(cartId, evt.CartId);
        Assert.Equal(productId, evt.ProductId);
    }

    [Fact]
    public void Constructor_Throws_WhenCartIdIsEmpty()
    {
        Assert.Throws<ArgumentException>(() => new CartProductRemovedEvent(Guid.Empty, "prod-123"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_Throws_WhenProductIdIsNullOrWhitespace(string badId)
    {
        Assert.Throws<ArgumentException>(() => new CartProductRemovedEvent(Guid.NewGuid(), badId));
    }
}