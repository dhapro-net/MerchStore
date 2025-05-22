using System;
using System.Threading;
using System.Threading.Tasks;
using MerchStore.Application.ShoppingCart.Commands;
using MerchStore.Application.ShoppingCart.Handlers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
namespace MerchStoreTest.Application.Handlers;
public class ClearCartCommandHandlerTest
{
    [Fact]
    public async Task Handle_ReturnsFailure_WhenCartIdIsEmpty()
    {
        var logger = new Mock<ILogger<ClearCartCommandHandler>>();
        var handler = new ClearCartCommandHandler(logger.Object);

        var command = new ClearCartCommand(Guid.Empty);
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task Handle_ReturnsSuccess_WhenCartIdIsValid()
    {
        var logger = new Mock<ILogger<ClearCartCommandHandler>>();
        var handler = new ClearCartCommandHandler(logger.Object);

        var command = new ClearCartCommand(Guid.NewGuid());
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
    }
}