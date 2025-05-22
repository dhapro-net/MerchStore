using System;
using System.Threading;
using System.Threading.Tasks;
using MerchStore.Application.ShoppingCart.Handlers;
using MerchStore.Application.ShoppingCart.Queries;
using MerchStore.Domain.ShoppingCart;
using Microsoft.Extensions.Logging;
using Xunit;
namespace MerchStoreTest.Application.Handlers;
public class GetCartSummaryQueryHandlerTest
{
    [Fact]
    public async Task Handle_ReturnsMappedSummary_WhenCartIsNotNull()
    {
        var logger = new LoggerFactory().CreateLogger<GetCartSummaryQueryHandler>();
        var handler = new GetCartSummaryQueryHandler(logger);

        var cart = Cart.Create(Guid.NewGuid(), new LoggerFactory().CreateLogger<Cart>());
        var query = new GetCartSummaryQuery(cart.CartId, cart);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(cart.CartId, result.CartId);
        Assert.Equal("SEK", result.TotalPrice.Currency);
    }

    [Fact]
    public async Task Handle_ReturnsNull_WhenCartIsNull()
    {
        var logger = new LoggerFactory().CreateLogger<GetCartSummaryQueryHandler>();
        var handler = new GetCartSummaryQueryHandler(logger);

        var query = new GetCartSummaryQuery(Guid.NewGuid(), null);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Null(result);
    }
}