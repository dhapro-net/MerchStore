using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MerchStore.Application.Catalog.Queries;
using MerchStore.Domain.Entities;
using MerchStore.Domain.Interfaces;
using MerchStore.Domain.ValueObjects;
using Moq;
using Xunit;

namespace MerchStoreTest.Application.Catalog.Queries.GetProductById
{
    public class GetProductByIdQueryHandlerTest
    {
        [Fact]
        public async Task Handle_ReturnsProductDto_WhenProductExists()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product(
                productId,
                "Test Product",
                "Test Description",
                "Test Category",
                "https://img.com/test.jpg",
                new Money(123, "SEK"),
                10
            );

            var repoMock = new Mock<IProductQueryRepository>();
            repoMock.Setup(r => r.GetProductByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var handler = new GetProductByIdQueryHandler(repoMock.Object);
            var query = new GetProductByIdQuery(productId);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(product.Id, result.Id);
            Assert.Equal(product.Name, result.Name);
            Assert.Equal(product.Description, result.Description);
            Assert.Equal(product.Price, result.Price);
            Assert.Equal(product.StockQuantity, result.StockQuantity);
            Assert.Equal(product.ImageUrl, result.ImageUrl.ToString());
        }

        [Fact]
        public async Task Handle_ReturnsProductDto_WithPlaceholderImage_WhenImageUrlIsNullOrEmpty()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product(
                productId,
                "Test Product",
                "Test Description",
                "Test Category",
                "https://example.com/placeholder.jpg",
                new Money(123, "SEK"),
                10
            );

            var repoMock = new Mock<IProductQueryRepository>();
            repoMock.Setup(r => r.GetProductByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var handler = new GetProductByIdQueryHandler(repoMock.Object);
            var query = new GetProductByIdQuery(productId);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("https://example.com/placeholder.jpg", result.ImageUrl.ToString());
        }

        [Fact]
        public async Task Handle_ThrowsKeyNotFoundException_WhenProductDoesNotExist()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var repoMock = new Mock<IProductQueryRepository>();
            repoMock.Setup(r => r.GetProductByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product)null);

            var handler = new GetProductByIdQueryHandler(repoMock.Object);
            var query = new GetProductByIdQuery(productId);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenRepositoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new GetProductByIdQueryHandler(null!));
        }
    }
}