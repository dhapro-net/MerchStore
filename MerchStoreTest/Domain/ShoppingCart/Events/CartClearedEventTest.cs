using System;
using MerchStore.Domain.ShoppingCart.Events;
using Xunit;
namespace MerchStoreTest.Domain.ShoppingCart.Events;
public class CartClearedEventTest
{
    [Fact]
    public void Constructor_SetsCartId()
    {
        var cartId = Guid.NewGuid();
        var evt = new CartClearedEvent(cartId);

        Assert.Equal(cartId, evt.CartId);
    }

    [Fact]
    public void Constructor_Throws_WhenCartIdIsEmpty()
    {
        Assert.Throws<ArgumentException>(() => new CartClearedEvent(Guid.Empty));
    }
}