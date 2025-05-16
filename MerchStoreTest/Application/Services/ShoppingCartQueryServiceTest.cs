using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MerchStore.Application.ShoppingCart.DTOs;
using MerchStore.Application.ShoppingCart.Queries;
using MerchStore.Application.ShoppingCart.Services;
using MerchStore.Domain.ShoppingCart;
using MerchStore.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace MerchStoreTest.Application.Services;

public class ShoppingCartQueryServiceTest
{
    private readonly Mock<IMediator> _mediatorMock = new();
    private readonly Mock<ILogger<ShoppingCartQueryService>> _loggerMock = new();
    private readonly Mock<ILogger<Cart>> _cartLoggerMock = new();
    private readonly ShoppingCartQueryService _service;

    public ShoppingCartQueryServiceTest()
    {
        _service = new ShoppingCartQueryService(
            _mediatorMock.Object,
            _loggerMock.Object,
            _cartLoggerMock.Object
        );
    }

    [Fact]
    public async Task GetCartAsync_ThrowsArgumentNullException_WhenQueryIsNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.GetCartAsync(null!, CancellationToken.None));
    }

    [Fact]
    public async Task GetCartAsync_ReturnsCartDto_WhenCartExists()
    {
        var cart = new CartDto { CartId = Guid.NewGuid(), TotalPrice = new Money(100, "SEK") };
        var query = new GetCartQuery(Cart.Create(cart.CartId, _cartLoggerMock.Object));
        _mediatorMock.Setup(m => m.Send(query, It.IsAny<CancellationToken>())).ReturnsAsync(cart);

        var result = await _service.GetCartAsync(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(cart.CartId, result.CartId);
    }

    [Fact]
    public async Task GetCartAsync_ReturnsEmptyCartDto_WhenCartIsNull()
    {
        var cartId = Guid.NewGuid();
        var cart = Cart.Create(cartId, _cartLoggerMock.Object);
        var query = new GetCartQuery(cart);
        _mediatorMock.Setup(m => m.Send(query, It.IsAny<CancellationToken>())).ReturnsAsync((CartDto)null);

        var result = await _service.GetCartAsync(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(cartId, result.CartId);
        Assert.Empty(result.Products);
    }

    [Fact]
    public async Task CalculateCartTotalAsync_ReturnsTotal_WhenCartExists()
    {
        var cartId = Guid.NewGuid();
        var cartDto = new CartDto
        {
            CartId = cartId,
            Products = new List<CartProductDto>
            {
                new CartProductDto { ProductId = "1", ProductName = "A", Quantity = 2, UnitPrice = new Money(10, "SEK") },
                new CartProductDto { ProductId = "2", ProductName = "B", Quantity = 1, UnitPrice = new Money(20, "SEK") }
            },
            TotalPrice = new Money(40, "SEK")
        };
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetCartQuery>(), default)).ReturnsAsync(cartDto);

        var result = await _service.CalculateCartTotalAsync(cartId);

        Assert.Equal(40, result.Amount);
        Assert.Equal("SEK", result.Currency);
    }

    [Fact]
    public async Task CalculateCartTotalAsync_ReturnsZero_WhenCartIsNull()
    {
        var cartId = Guid.NewGuid();
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetCartQuery>(), default)).ReturnsAsync((CartDto)null);

        var result = await _service.CalculateCartTotalAsync(cartId);

        Assert.Equal(0, result.Amount);
        Assert.Equal("SEK", result.Currency);
    }

    [Fact]
    public async Task GetCartSummaryAsync_ThrowsArgumentNullException_WhenQueryIsNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.GetCartSummaryAsync(null!, CancellationToken.None));
    }

    [Fact]
    public async Task GetCartSummaryAsync_ReturnsSummary()
    {
        var summary = new CartSummaryDto
        {
            TotalPrice = new Money(0, "SEK")
        };
        var cartId = Guid.NewGuid();
        var cart = Cart.Create(cartId, _cartLoggerMock.Object);
        var query = new GetCartSummaryQuery(cartId, cart);
        _mediatorMock.Setup(m => m.Send(query, It.IsAny<CancellationToken>())).ReturnsAsync(summary);

        var result = await _service.GetCartSummaryAsync(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(summary, result);
    }

    [Fact]
    public async Task HasProductsAsync_ReturnsTrue_WhenCartHasProducts()
    {
        var cartId = Guid.NewGuid();
        var cartDto = new CartDto
        {
            CartId = cartId,
            Products = new List<CartProductDto>
            {
                new CartProductDto { ProductId = "1", ProductName = "A", Quantity = 1, UnitPrice = new Money(10, "SEK") }
            },
            TotalPrice = new Money(10, "SEK")
        };
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetCartQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(cartDto);

        var result = await _service.HasProductsAsync(cartId, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task HasProductsAsync_ReturnsFalse_WhenCartIsNull()
    {
        var cartId = Guid.NewGuid();
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetCartQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync((CartDto)null);

        var result = await _service.HasProductsAsync(cartId, CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task ProductCountAsync_ReturnsCorrectCount()
    {
        var cartId = Guid.NewGuid();
        var cartDto = new CartDto
        {
            CartId = cartId,
            Products = new List<CartProductDto>
            {
                new CartProductDto { ProductId = "1", ProductName = "A", Quantity = 2, UnitPrice = new Money(10, "SEK") },
                new CartProductDto { ProductId = "2", ProductName = "B", Quantity = 3, UnitPrice = new Money(20, "SEK") }
            },
            TotalPrice = new Money(70, "SEK")
        };
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetCartQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(cartDto);

        var result = await _service.ProductCountAsync(cartId, CancellationToken.None);

        Assert.Equal(5, result);
    }

    [Fact]
    public async Task ProductCountAsync_ReturnsZero_WhenCartIsNull()
    {
        var cartId = Guid.NewGuid();
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetCartQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync((CartDto)null);

        var result = await _service.ProductCountAsync(cartId, CancellationToken.None);

        Assert.Equal(0, result);
    }

    [Fact]
    public async Task ContainsProductAsync_ThrowsArgumentException_WhenProductIdIsNullOrEmpty()
    {
        var cartId = Guid.NewGuid();
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.ContainsProductAsync(cartId, null!, CancellationToken.None));
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.ContainsProductAsync(cartId, "", CancellationToken.None));
    }

    [Fact]
    public async Task ContainsProductAsync_ReturnsTrue_WhenProductExists()
    {
        var cartId = Guid.NewGuid();
        var cartDto = new CartDto
        {
            CartId = cartId,
            Products = new List<CartProductDto>
            {
                new CartProductDto { ProductId = "prod-1", ProductName = "A", Quantity = 1, UnitPrice = new Money(10, "SEK") }
            },
            TotalPrice = new Money(10, "SEK")
        };
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetCartQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(cartDto);

        var result = await _service.ContainsProductAsync(cartId, "prod-1", CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task ContainsProductAsync_ReturnsFalse_WhenProductDoesNotExist()
    {
        var cartId = Guid.NewGuid();
        var cartDto = new CartDto
        {
            CartId = cartId,
            Products = new List<CartProductDto>(),
            TotalPrice = new Money(0, "SEK")
        };
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetCartQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(cartDto);

        var result = await _service.ContainsProductAsync(cartId, "prod-1", CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task GetProductAsync_ThrowsArgumentException_WhenProductIdIsNullOrEmpty()
    {
        var cartId = Guid.NewGuid();
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.GetProductAsync(cartId, null!, CancellationToken.None));
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.GetProductAsync(cartId, "", CancellationToken.None));
    }

    [Fact]
    public async Task GetProductAsync_ReturnsProduct_WhenExists()
    {
        var cartId = Guid.NewGuid();
        var product = new CartProductDto { ProductId = "prod-1", ProductName = "A", Quantity = 1, UnitPrice = new Money(10, "SEK") };
        var cartDto = new CartDto
        {
            CartId = cartId,
            Products = new List<CartProductDto> { product },
            TotalPrice = new Money(10, "SEK")
        };
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetCartQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(cartDto);

        var result = await _service.GetProductAsync(cartId, "prod-1", CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("prod-1", result.ProductId);
    }

    [Fact]
    public async Task GetProductAsync_ReturnsNull_WhenCartIsNull()
    {
        var cartId = Guid.NewGuid();
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetCartQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync((CartDto)null);

        var result = await _service.GetProductAsync(cartId, "prod-1", CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetProductAsync_ReturnsNull_WhenProductNotFound()
    {
        var cartId = Guid.NewGuid();
        var cartDto = new CartDto
        {
            CartId = cartId,
            Products = new List<CartProductDto>(),
            TotalPrice = new Money(0, "SEK")
        };
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetCartQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(cartDto);

        var result = await _service.GetProductAsync(cartId, "prod-1", CancellationToken.None);

        Assert.Null(result);
    }
}