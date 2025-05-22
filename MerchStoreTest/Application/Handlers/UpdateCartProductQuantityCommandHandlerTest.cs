using System;
using System.Threading;
using System.Threading.Tasks;
using MerchStore.Application.ShoppingCart.Commands;
using MerchStore.Domain.ShoppingCart;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
namespace MerchStoreTest.Application.Handlers;
public class UpdateCartProductQuantityCommandHandlerTest
{
    [Fact]
    public async Task Handle_ReturnsFailure_WhenCartIsNull()
    {
        var logger = new Mock<ILogger<UpdateCartProductQuantityCommandHandler>>();
        var handler = new UpdateCartProductQuantityCommandHandler(logger.Object);

        var command = new UpdateCartProductQuantityCommand(null!, "prod", 1);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ReturnsFailure_WhenProductIdIsNullOrEmpty()
    {
        var logger = new Mock<ILogger<UpdateCartProductQuantityCommandHandler>>();
        var handler = new UpdateCartProductQuantityCommandHandler(logger.Object);

        var cart = new Mock<Cart>(Guid.NewGuid(), null, DateTime.UtcNow, DateTime.UtcNow).Object;
        var command = new UpdateCartProductQuantityCommand(cart, "", 1);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
    }
}