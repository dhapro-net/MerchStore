using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MerchStore.Application.Services.Interfaces;
using MerchStore.Domain.ValueObjects;
using MerchStore.WebUI.Controllers.Api.Products;
using MerchStore.WebUI.Models.Api.Basic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace MerchStore.Tests.WebUI.Controllers.Api
{
    public class BasicProductsApiControllerTest
    {
        private readonly Mock<ICatalogService> _catalogServiceMock;
        private readonly BasicProductsApiController _controller;

        public BasicProductsApiControllerTest()
        {
            _catalogServiceMock = new Mock<ICatalogService>();
            _controller = new BasicProductsApiController(_catalogServiceMock.Object);
        }

        #region GetAll

        [Fact]
        public async Task GetAll_ReturnsOk_WithProductList()
        {
            // Arrange
            var products = new List<dynamic>
            {
                new {
                    Id = Guid.NewGuid(),
                    Name = "Product1",
                    Description = "Desc1",
                    Price = new Money(100, "SEK"),
                    Currency = "SEK",
                    ImageUrl = new Uri("http://img.com/1"),
                    StockQuantity = 5
                },
                new {
                    Id = Guid.NewGuid(),
                    Name = "Product2",
                    Description = "Desc2",
                    Price = new Money(200, "SEK"),
                    Currency = "SEK",
                    ImageUrl = new Uri("http://img.com/2"),
                    StockQuantity = 10
                }
            };

            _catalogServiceMock.Setup(s => s.GetAllProductsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(products);

            // Act
            var result = await _controller.GetAll(CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<BasicProductDto>>(okResult.Value);
            Assert.Equal(2, dtos.Count());
            Assert.Contains(dtos, d => d.Name == "Product1");
            Assert.Contains(dtos, d => d.Name == "Product2");
        }

        [Fact]
        public async Task GetAll_ReturnsEmptyList_WhenNoProducts()
        {
            // Arrange
            _catalogServiceMock.Setup(s => s.GetAllProductsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<dynamic>());

            // Act
            var result = await _controller.GetAll(CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var dtos = Assert.IsAssignableFrom<IEnumerable<BasicProductDto>>(okResult.Value);
            Assert.Empty(dtos);
        }

        [Fact]
        public async Task GetAll_Returns500_WhenServiceThrows()
        {
            // Arrange
            _catalogServiceMock.Setup(s => s.GetAllProductsAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("fail"));

            // Act
            var result = await _controller.GetAll(CancellationToken.None);

            // Assert
            var errorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, errorResult.StatusCode);
            Assert.Contains("An error occurred", errorResult.Value.ToString());
        }

        #endregion

        #region GetById

        [Fact]
        public async Task GetById_ReturnsOk_WhenProductExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var product = new
            {
                Id = id,
                Name = "ProductX",
                Description = "DescX",
                Price = new Money(123, "SEK"),
                Currency = "SEK",
                ImageUrl = new Uri("http://img.com/x"),
                StockQuantity = 7
            };

            _catalogServiceMock.Setup(s => s.GetProductByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            // Act
            var result = await _controller.GetById(id, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<BasicProductDto>(okResult.Value);
            Assert.Equal(id, dto.Id);
            Assert.Equal("ProductX", dto.Name);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var id = Guid.NewGuid();
            _catalogServiceMock.Setup(s => s.GetProductByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((object)null);

            // Act
            var result = await _controller.GetById(id, CancellationToken.None);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("not found", notFound.Value.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task GetById_Returns500_WhenServiceThrows()
        {
            // Arrange
            var id = Guid.NewGuid();
            _catalogServiceMock.Setup(s => s.GetProductByIdAsync(id, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("fail"));

            // Act
            var result = await _controller.GetById(id, CancellationToken.None);

            // Assert
            var errorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, errorResult.StatusCode);
            Assert.Contains("An error occurred", errorResult.Value.ToString());
        }

        #endregion
    }
}