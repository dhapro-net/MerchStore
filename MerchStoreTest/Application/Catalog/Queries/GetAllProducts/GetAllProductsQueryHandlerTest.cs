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

namespace MerchStoreTest.Application.Catalog.Queries.GetAllProducts;
public class GetAllProductsQueryHandlerTest
{
    [Fact]
    public async Task Handle_ReturnsMappedProductDtos_WhenProductsExist()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product(
                Guid.NewGuid(),
                "Product1",
                "Description1",
                "Category1",
                "https://img.com/1.jpg",
                new Money(100, "SEK"),
                5
            ),
            new Product(
                Guid.NewGuid(),
                "Product2",
                "Description2",
                "Category2",
                "https://example.com/placeholder.jpg",
                new Money(200, "SEK"),
                10
            )
        };

        var repoMock = new Mock<IProductQueryRepository>();
        repoMock.Setup(r => r.GetAllProductsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        var handler = new GetAllProductsQueryHandler(repoMock.Object);

        // Act
        var result = await handler.Handle(new GetAllProductsQuery(), CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(products[0].Id, result[0].Id);
        Assert.Equal(products[1].Id, result[1].Id);
        Assert.Equal("Product1", result[0].Name);
        Assert.Equal("Product2", result[1].Name);
        Assert.Equal("https://img.com/1.jpg", result[0].ImageUrl.ToString());
        Assert.Equal("https://example.com/placeholder.jpg", result[1].ImageUrl.ToString());
    }

    [Fact]
    public async Task Handle_ReturnsEmptyList_WhenNoProducts()
    {
        // Arrange
        var repoMock = new Mock<IProductQueryRepository>();
        repoMock.Setup(r => r.GetAllProductsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product>());

        var handler = new GetAllProductsQueryHandler(repoMock.Object);

        // Act
        var result = await handler.Handle(new GetAllProductsQuery(), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenRepositoryIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new GetAllProductsQueryHandler(null!));
    }
}