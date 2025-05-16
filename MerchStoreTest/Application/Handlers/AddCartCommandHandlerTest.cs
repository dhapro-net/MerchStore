using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MerchStore.Application.ShoppingCart.Commands;
using MerchStore.Application.ShoppingCart.Handlers;
using MerchStore.Domain.ShoppingCart;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace MerchStoreTest.Application.Handlers;
public class AddCartCommandHandlerTest
{
    [Fact]
    public async Task Handle_ThrowsArgumentNullException_WhenCartIsNull()
    {
        var logger = new Mock<ILogger<AddCartCommandHandler>>();
        var handler = new AddCartCommandHandler(logger.Object);

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
        {
            var command = new AddCartCommand(null!, CancellationToken.None);
            return handler.Handle(command, CancellationToken.None);
        });
    }

[Fact]
public async Task Handle_ReturnsUnit_WhenCartIsValid()
{
    var logger = new Mock<ILogger<AddCartCommandHandler>>();
    var handler = new AddCartCommandHandler(logger.Object);

    // Create a real Cart instance with valid arguments
    var cart = new Cart(
        Guid.NewGuid(),
        new List<CartProduct>(), // or add valid CartProduct(s) if needed
        DateTime.UtcNow,
        DateTime.UtcNow
    );
    var command = new AddCartCommand(cart, CancellationToken.None);

    var result = await handler.Handle(command, CancellationToken.None);

    Assert.Equal(Unit.Value, result);
}
}