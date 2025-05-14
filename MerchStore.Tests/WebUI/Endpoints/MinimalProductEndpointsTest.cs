using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MerchStore.Application.Services.Interfaces;
using MerchStore.Domain.ValueObjects;
using MerchStore.WebUI.Models.Api.Minimal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using Xunit;

namespace MerchStore.Tests.WebUI.Endpoints
{
    public class MinimalProductEndpointsTest
    {
        #region GetAllProducts

        [Fact]
        public async Task GetAllProducts_ReturnsOk_WithProductList()
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
                }
            };
            var serviceMock = new Mock<ICatalogService>();
            serviceMock.Setup(s => s.GetAllProductsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(products);

            // Act
            var result = await InvokeGetAllProducts(serviceMock.Object);

            // Assert
            var okResult = Assert.IsType<Ok<List<MinimalProductResponse>>>(result);
            Assert.Single(okResult.Value);
            Assert.Equal("Product1", okResult.Value[0].Name);
        }

        [Fact]
        public async Task GetAllProducts_ReturnsOk_WithEmptyList()
        {
            // Arrange
            var serviceMock = new Mock<ICatalogService>();
            serviceMock.Setup(s => s.GetAllProductsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<dynamic>());

            // Act
            var result = await InvokeGetAllProducts(serviceMock.Object);

            // Assert
            var okResult = Assert.IsType<Ok<List<MinimalProductResponse>>>(result);
            Assert.Empty(okResult.Value);
        }

        [Fact]
        public async Task GetAllProducts_ReturnsProblem_OnException()
        {
            // Arrange
            var serviceMock = new Mock<ICatalogService>();
            serviceMock.Setup(s => s.GetAllProductsAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("fail"));

            // Act
            var result = await InvokeGetAllProducts(serviceMock.Object);

            // Assert
            var problemResult = Assert.IsType<ProblemHttpResult>(result);
            Assert.Contains("An error occurred", problemResult.ProblemDetails.Detail);
        }

        private static Task<IResult> InvokeGetAllProducts(ICatalogService service)
        {
            // Use reflection to call the private static method
            var method = typeof(MerchStore.WebUI.Endpoints.MinimalProductEndpoints)
                .GetMethod("GetAllProducts", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            return (Task<IResult>)method.Invoke(null, new object[] { service, CancellationToken.None });
        }

        #endregion

        #region GetProductById

        [Fact]
        public async Task GetProductById_ReturnsOk_WhenProductExists()
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
            var serviceMock = new Mock<ICatalogService>();
            serviceMock.Setup(s => s.GetProductByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            // Act
            var result = await InvokeGetProductById(id, serviceMock.Object);

            // Assert
            var okResult = Assert.IsType<Ok<MinimalProductResponse>>(result);
            Assert.Equal(id, okResult.Value.Id);
            Assert.Equal("ProductX", okResult.Value.Name);
        }

        [Fact]
        public async Task GetProductById_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var id = Guid.NewGuid();
            var serviceMock = new Mock<ICatalogService>();
            serviceMock.Setup(s => s.GetProductByIdAsync(id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((object)null);

            // Act
            var result = await InvokeGetProductById(id, serviceMock.Object);

            // Assert
            var notFoundResult = Assert.IsType<NotFound<string>>(result);
            Assert.Contains("not found", notFoundResult.Value, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task GetProductById_ReturnsProblem_OnException()
        {
            // Arrange
            var id = Guid.NewGuid();
            var serviceMock = new Mock<ICatalogService>();
            serviceMock.Setup(s => s.GetProductByIdAsync(id, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("fail"));

            // Act
            var result = await InvokeGetProductById(id, serviceMock.Object);

            // Assert
            var problemResult = Assert.IsType<ProblemHttpResult>(result);
            Assert.Contains("An error occurred", problemResult.ProblemDetails.Detail);
        }

        private static Task<IResult> InvokeGetProductById(Guid id, ICatalogService service)
        {
            // Use reflection to call the private static method
            var method = typeof(MerchStore.WebUI.Endpoints.MinimalProductEndpoints)
                .GetMethod("GetProductById", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            return (Task<IResult>)method.Invoke(null, new object[] { id, service, CancellationToken.None });
        }

        #endregion
    }
}