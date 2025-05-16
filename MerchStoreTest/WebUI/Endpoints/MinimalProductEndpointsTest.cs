using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MerchStore.Application.Services.Interfaces;
using MerchStore.Domain.Entities; // <-- Make sure this is the correct namespace for Product
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
            var products = new List<Product>
            {
new Product(
    Guid.NewGuid(),
    "Product1",
    "Desc1",
    "Category1",                  // category (string)
    "http://img.com/1.jpg",       // imageUrl (string)
    new Money(100, "SEK"),        // price (Money)
    5                             // stockQuantity (int)
)
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
                .ReturnsAsync(new List<Product>());

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
            if (method == null)
                throw new InvalidOperationException("GetAllProducts method not found on MinimalProductEndpoints.");
            var result = method.Invoke(null, new object[] { service, CancellationToken.None });
            if (result == null)
                throw new InvalidOperationException("GetAllProducts invocation returned null.");
            return (Task<IResult>)result;
        }

        #endregion

        #region GetProductById

        [Fact]
        public async Task GetProductById_ReturnsOk_WhenProductExists()
        {
            // Arrange
            var id = Guid.NewGuid();
            var product = new Product(
                id,
                "ProductX",
                "DescX",
                "CategoryX",
                "http://img.com/x.jpg",
                new Money(123, "SEK"),
                7
            );
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
                .ReturnsAsync((Product)null);

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
            if (method == null)
                throw new InvalidOperationException("GetProductById method not found on MinimalProductEndpoints.");
            var result = method.Invoke(null, new object[] { id, service, CancellationToken.None });
            if (result == null)
                throw new InvalidOperationException("GetProductById invocation returned null.");
            return (Task<IResult>)result;
        }

        #endregion

    }
}