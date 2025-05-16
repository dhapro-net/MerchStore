using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MerchStore.Application.ShoppingCart.Commands;
using MerchStore.Application.ShoppingCart.Handlers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
namespace MerchStoreTest.Application.Handlers;
public class DeleteCartCommandHandlerTest
{
    [Fact]
    public async Task Handle_ThrowsArgumentException_WhenCartIdIsEmpty()
    {
        var logger = new Mock<ILogger<DeleteCartCommandHandler>>();
        var handler = new DeleteCartCommandHandler(logger.Object);

        var command = new DeleteCartCommand(Guid.Empty);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ReturnsUnit_WhenCartIdIsValid()
    {
        var logger = new Mock<ILogger<DeleteCartCommandHandler>>();
        var handler = new DeleteCartCommandHandler(logger.Object);

        var command = new DeleteCartCommand(Guid.NewGuid());
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(Unit.Value, result);
    }
}