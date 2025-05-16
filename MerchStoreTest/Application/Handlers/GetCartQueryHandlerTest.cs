using System;
using System.Threading;
using System.Threading.Tasks;
using MerchStore.Application.ShoppingCart.Queries;
using MerchStore.Domain.ShoppingCart;
using Microsoft.Extensions.Logging;
using Xunit;
namespace MerchStoreTest.Application.Handlers;
public class GetCartQueryHandlerTest
{
    [Fact]
    public async Task Handle_ReturnsMappedCartDto_WhenCartIsNotNull()
    {
        var logger = new LoggerFactory().CreateLogger<GetCartQueryHandler>();
        var handler = new GetCartQueryHandler(logger);

        var cart = Cart.Create(Guid.NewGuid(), new LoggerFactory().CreateLogger<Cart>());
        var query = new GetCartQuery(cart);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(cart.CartId, result.CartId);
    }

    [Fact]
    public async Task Handle_ReturnsNull_WhenCartIsNull()
    {
        var logger = new LoggerFactory().CreateLogger<GetCartQueryHandler>();
        var handler = new GetCartQueryHandler(logger);

        var query = new GetCartQuery(null);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Null(result);
    }
}