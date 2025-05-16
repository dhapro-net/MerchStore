using System;
using System.Threading;
using System.Threading.Tasks;
using MerchStore.Application.ShoppingCart.Commands;
using MerchStore.Domain.ShoppingCart;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
namespace MerchStoreTest.Application.Handlers;
public class RemoveProductFromCartCommandHandlerTest
{
    [Fact]
    public void Handle_ThrowsArgumentNullException_WhenCartIsNull()
    {
        var logger = new Mock<ILogger<RemoveProductFromCartCommandHandler>>();
        var handler = new RemoveProductFromCartCommandHandler(logger.Object);

        Assert.Throws<ArgumentNullException>(() =>
        {
            var command = new RemoveProductFromCartCommand(null!, "prod");
            // The exception is thrown in the constructor, so no need to call handler.Handle
        });
    }

    [Fact]
    public async Task Handle_ReturnsFailure_WhenProductIdIsNullOrEmpty()
    {
        var logger = new Mock<ILogger<RemoveProductFromCartCommandHandler>>();
        var handler = new RemoveProductFromCartCommandHandler(logger.Object);

        var cart = new Mock<Cart>(Guid.NewGuid(), null, DateTime.UtcNow, DateTime.UtcNow).Object;
        var command = new RemoveProductFromCartCommand(cart, "");
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
    }
}