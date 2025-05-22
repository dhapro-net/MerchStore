using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MerchStore.Application.ShoppingCart.Commands;
using MerchStore.Application.ShoppingCart.DTOs;
using MerchStore.Application.ShoppingCart.Queries;
using MerchStore.Application.ShoppingCart.Services;
using MerchStore.Domain.ShoppingCart;
using MerchStore.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace MerchStoreTest.Application.Services;

public class ShoppingCartCommandServiceTest
{
    private readonly Mock<IMediator> _mediatorMock = new();
    private readonly Mock<ILogger<ShoppingCartCommandService>> _loggerMock = new();
    private readonly Mock<ILogger<Cart>> _cartLoggerMock = new();
    private readonly ShoppingCartCommandService _service;

    public ShoppingCartCommandServiceTest()
    {
        _service = new ShoppingCartCommandService(
            _mediatorMock.Object,
            _loggerMock.Object,
            _cartLoggerMock.Object
        );
    }

    [Fact]
    public async Task AddAsync_ThrowsArgumentNullException_WhenCartIsNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.AddAsync(null!, CancellationToken.None));
    }

    [Fact]
    public async Task AddAsync_SendsAddCartCommand()
    {
        var cart = Cart.Create(Guid.NewGuid(), _cartLoggerMock.Object);

        _mediatorMock.Setup(m => m.Send(It.IsAny<AddCartCommand>(), It.IsAny<CancellationToken>()))
    .Returns(Task.FromResult(Unit.Value))
    .Verifiable();

        await _service.AddAsync(cart, CancellationToken.None);

        _mediatorMock.Verify(m => m.Send(It.IsAny<AddCartCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddProductToCartAsync_ThrowsArgumentException_WhenCartIdIsEmpty()
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.AddProductToCartAsync(Guid.Empty, "prod", 1, CancellationToken.None));
    }

    [Fact]
    public async Task AddProductToCartAsync_ThrowsArgumentException_WhenProductIdIsNullOrEmpty()
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.AddProductToCartAsync(Guid.NewGuid(), null!, 1, CancellationToken.None));
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.AddProductToCartAsync(Guid.NewGuid(), "", 1, CancellationToken.None));
    }

    [Fact]
    public async Task AddProductToCartAsync_ThrowsArgumentException_WhenQuantityIsZeroOrNegative()
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.AddProductToCartAsync(Guid.NewGuid(), "prod", 0, CancellationToken.None));
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.AddProductToCartAsync(Guid.NewGuid(), "prod", -1, CancellationToken.None));
    }

    [Fact]
    public async Task AddProductToCartAsync_ReturnsFalse_WhenResultIsFailure()
    {
        var cartId = Guid.NewGuid();
        _mediatorMock.Setup(m => m.Send(It.IsAny<AddProductToCartCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(MerchStore.Application.Common.Result<bool>.Failure("fail"));

        var result = await _service.AddProductToCartAsync(cartId, "prod", 1, CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task AddProductToCartAsync_ReturnsTrue_WhenResultIsSuccess()
    {
        var cartId = Guid.NewGuid();
        _mediatorMock.Setup(m => m.Send(It.IsAny<AddProductToCartCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(MerchStore.Application.Common.Result<bool>.Success(true));

        var result = await _service.AddProductToCartAsync(cartId, "prod", 1, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task RemoveProductFromCartAsync_ThrowsArgumentException_WhenCartIdIsEmpty()
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.RemoveProductFromCartAsync(Guid.Empty, "prod", CancellationToken.None));
    }

    [Fact]
    public async Task RemoveProductFromCartAsync_ThrowsArgumentException_WhenProductIdIsNullOrEmpty()
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.RemoveProductFromCartAsync(Guid.NewGuid(), null!, CancellationToken.None));
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.RemoveProductFromCartAsync(Guid.NewGuid(), "", CancellationToken.None));
    }

    [Fact]
    public async Task RemoveProductFromCartAsync_ReturnsFalse_WhenCartDtoIsNull()
    {
        var cartId = Guid.NewGuid();
        _mediatorMock
    .Setup(m => m.Send<CartDto>(It.IsAny<GetCartQuery>(), It.IsAny<CancellationToken>()))
    .ReturnsAsync((CartDto)null);

        var result = await _service.RemoveProductFromCartAsync(cartId, "prod", CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task RemoveProductFromCartAsync_ReturnsFalse_WhenResultIsFailure()
    {
        var cartId = Guid.NewGuid();
        _mediatorMock.Setup(m => m.Send<CartDto>(It.IsAny<GetCartQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CartDto { TotalPrice = new Money(0, "SEK") }); // dummy cartDto with required TotalPrice
        _mediatorMock.Setup(m => m.Send(It.IsAny<RemoveProductFromCartCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(MerchStore.Application.Common.Result<bool>.Failure("fail"));

        var result = await _service.RemoveProductFromCartAsync(cartId, "prod", CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task RemoveProductFromCartAsync_ReturnsTrue_WhenResultIsSuccess()
    {
        var cartId = Guid.NewGuid();
        _mediatorMock
            .Setup(m => m.Send<CartDto>(It.IsAny<GetCartQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CartDto { TotalPrice = new Money(0, "SEK") }); // Provide a non-null CartDto
        _mediatorMock.Setup(m => m.Send(It.IsAny<RemoveProductFromCartCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(MerchStore.Application.Common.Result<bool>.Success(true));

        var result = await _service.RemoveProductFromCartAsync(cartId, "prod", CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task UpdateProductQuantityAsync_ThrowsArgumentException_WhenCartIdIsEmpty()
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.UpdateProductQuantityAsync(Guid.Empty, "prod", 1, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateProductQuantityAsync_ThrowsArgumentException_WhenProductIdIsNullOrEmpty()
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.UpdateProductQuantityAsync(Guid.NewGuid(), null!, 1, CancellationToken.None));
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.UpdateProductQuantityAsync(Guid.NewGuid(), "", 1, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateProductQuantityAsync_ThrowsArgumentException_WhenQuantityIsZeroOrNegative()
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.UpdateProductQuantityAsync(Guid.NewGuid(), "prod", 0, CancellationToken.None));
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.UpdateProductQuantityAsync(Guid.NewGuid(), "prod", -1, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateProductQuantityAsync_ReturnsFalse_WhenCartDtoIsNull()
    {
        var cartId = Guid.NewGuid();
        _mediatorMock
            .Setup(m => m.Send<CartDto>(It.IsAny<GetCartQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CartDto)null);

        var result = await _service.UpdateProductQuantityAsync(cartId, "prod", 1, CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task UpdateProductQuantityAsync_ReturnsFalse_WhenResultIsFailure()
    {
        var cartId = Guid.NewGuid();
        _mediatorMock
            .Setup(m => m.Send<CartDto>(It.IsAny<GetCartQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CartDto { TotalPrice = new Money(0, "SEK") }); // or use a suitable CartDto for your test

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<UpdateCartProductQuantityCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(MerchStore.Application.Common.Result<bool>.Failure("fail"));

        var result = await _service.UpdateProductQuantityAsync(cartId, "prod", 1, CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task UpdateProductQuantityAsync_ReturnsTrue_WhenResultIsSuccess()
    {
        var cartId = Guid.NewGuid();
        _mediatorMock.Setup(m => m.Send<CartDto>(It.IsAny<GetCartQuery>(), It.IsAny<CancellationToken>()))
    .ReturnsAsync(new CartDto { TotalPrice = new Money(0, "SEK") }); // or a suitable CartDto for your test
        _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateCartProductQuantityCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(MerchStore.Application.Common.Result<bool>.Success(true));

        var result = await _service.UpdateProductQuantityAsync(cartId, "prod", 1, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task ClearCartAsync_ThrowsArgumentException_WhenCartIdIsEmpty()
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.ClearCartAsync(Guid.Empty, CancellationToken.None));
    }

    [Fact]
    public async Task ClearCartAsync_ReturnsFalse_WhenResultIsFailure()
    {
        var cartId = Guid.NewGuid();
        _mediatorMock.Setup(m => m.Send(It.IsAny<ClearCartCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(MerchStore.Application.Common.Result<bool>.Failure("fail"));

        var result = await _service.ClearCartAsync(cartId, CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task ClearCartAsync_ReturnsTrue_WhenResultIsSuccess()
    {
        var cartId = Guid.NewGuid();
        _mediatorMock.Setup(m => m.Send(It.IsAny<ClearCartCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(MerchStore.Application.Common.Result<bool>.Success(true));

        var result = await _service.ClearCartAsync(cartId, CancellationToken.None);

        Assert.True(result);
    }
}