using System;
using System.Threading;
using System.Threading.Tasks;
using MerchStore.Application.ShoppingCart.Commands;
using MerchStore.Application.ShoppingCart.Handlers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
namespace MerchStoreTest.Application.Handlers;
public class AddProductToCartCommandHandlerTest
{
    [Fact]
    public async Task Handle_ReturnsFailure_WhenProductIdIsNullOrEmpty()
    {
        var logger = new Mock<ILogger<AddProductToCartCommandHandler>>();
        var handler = new AddProductToCartCommandHandler(logger.Object);

        var command = new AddProductToCartCommand(Guid.NewGuid(), null!, 1, CancellationToken.None);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ReturnsFailure_WhenQuantityIsZeroOrNegative()
    {
        var logger = new Mock<ILogger<AddProductToCartCommandHandler>>();
        var handler = new AddProductToCartCommandHandler(logger.Object);

        var command = new AddProductToCartCommand(Guid.NewGuid(), "prod", 0, CancellationToken.None);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ReturnsSuccess_WhenValid()
    {
        var logger = new Mock<ILogger<AddProductToCartCommandHandler>>();
        var handler = new AddProductToCartCommandHandler(logger.Object);

        var command = new AddProductToCartCommand(Guid.NewGuid(), "prod", 2, CancellationToken.None);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
    }
}