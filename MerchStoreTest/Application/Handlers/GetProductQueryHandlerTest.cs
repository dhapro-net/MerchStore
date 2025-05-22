using System;
using System.Threading;
using System.Threading.Tasks;
using MerchStore.Application.ShoppingCart.Handlers;
using MerchStore.Application.ShoppingCart.Queries;
using MerchStore.Domain.ShoppingCart;
using MerchStore.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace MerchStoreTest.Application.Handlers
{
    public class GetProductQueryHandlerTest
    {
        [Fact]
        public async Task Handle_ReturnsMappedProduct_WhenProductExists()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<GetProductQueryHandler>>();
            var cartLoggerMock = new Mock<ILogger<Cart>>();
            var handler = new GetProductQueryHandler(loggerMock.Object);

            var productId = "prod-1";
            var expectedProduct = new CartProduct(productId, "Test Product", new Money(100, "SEK"), 1);

            var cart = Cart.Create(Guid.NewGuid(), cartLoggerMock.Object);
            cart.Products.Add(expectedProduct);

            var query = new GetProductQuery(cart, productId);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productId, result.ProductId);
        }

        [Fact]
        public async Task Handle_ReturnsNull_WhenCartIsNull()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<GetProductQueryHandler>>();
            var handler = new GetProductQueryHandler(loggerMock.Object);

            var query = new GetProductQuery(null, "prod-1");

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }
    }
}