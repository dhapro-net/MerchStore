using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MerchStore.Application.Services.Implementations;
using MerchStore.Domain.Entities;
using MerchStore.Domain.Interfaces;
using Moq;
using Xunit;
namespace MerchStoreTest.Application.Services.Implementations;
public class CatalogServiceTest
{
    private readonly Mock<IProductQueryRepository> _repoMock;
    private readonly CatalogService _service;

    public CatalogServiceTest()
    {
        _repoMock = new Mock<IProductQueryRepository>();
        _service = new CatalogService(_repoMock.Object);
    }

    [Fact]
    public async Task GetAllProductsAsync_ReturnsProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product(Guid.NewGuid(), "Product1", "Desc1", "Cat1", "http://img.com/1.jpg", new MerchStore.Domain.ValueObjects.Money(100, "SEK"), 5),
            new Product(Guid.NewGuid(), "Product2", "Desc2", "Cat2", "http://img.com/2.jpg", new MerchStore.Domain.ValueObjects.Money(200, "SEK"), 10)
        };
        _repoMock.Setup(r => r.GetAllProductsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        // Act
        var result = await _service.GetAllProductsAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, ((List<Product>)result).Count);
    }

    [Fact]
    public async Task GetAllProductsAsync_ReturnsEmpty_WhenNoProducts()
    {
        // Arrange
        _repoMock.Setup(r => r.GetAllProductsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product>());

        // Act
        var result = await _service.GetAllProductsAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetProductByIdAsync_ReturnsProduct_WhenExists()
    {
        // Arrange
        var id = Guid.NewGuid();
        var product = new Product(id, "Product1", "Desc1", "Cat1", "http://img.com/1.jpg", new MerchStore.Domain.ValueObjects.Money(100, "SEK"), 5);
        _repoMock.Setup(r => r.GetProductByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act
        var result = await _service.GetProductByIdAsync(id, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
    }

    [Fact]
    public async Task GetProductByIdAsync_ReturnsNull_WhenNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repoMock.Setup(r => r.GetProductByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product)null);

        // Act
        var result = await _service.GetProductByIdAsync(id, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}