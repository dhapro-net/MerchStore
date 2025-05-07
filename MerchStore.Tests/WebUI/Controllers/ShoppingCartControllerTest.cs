using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using MerchStore.Application.Services.Interfaces;
using MerchStore.WebUI.Controllers;
using MerchStore.WebUI.Models.ShoppingCart;

//Copilot generated tests.
namespace MerchStore.WebUI.Tests.Controllers
{
    public class ShoppingCartControllerTests
    {
        private readonly Mock<IShoppingCartQueryService> _mockQueryService;
        private readonly Mock<IShoppingCartService> _mockService;
        private readonly Mock<ILogger<ShoppingCartController>> _mockLogger;
        private readonly ShoppingCartController _controller;

        public ShoppingCartControllerTests()
        {
            _mockQueryService = new Mock<IShoppingCartQueryService>();
            _mockService = new Mock<IShoppingCartService>();
            _mockLogger = new Mock<ILogger<ShoppingCartController>>();
            _controller = new ShoppingCartController(
                _mockQueryService.Object,
                _mockService.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task Index_ReturnsViewWithViewModel_WhenCartExists()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var cartDto = new CartDto
            {
                Id = cartId,
                TotalPrice = 100,
                TotalItems = 2
            };
            _mockQueryService.Setup(s => s.GetCartAsync(cartId)).ReturnsAsync(cartDto);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<CartDto>(viewResult.Model);
            Assert.Equal(cartDto, viewResult.Model);
        }

        [Fact]
        public async Task Index_ReturnsViewWithEmptyCart_WhenCartIsEmpty()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var cartDto = new CartDto
            {
                Id = cartId,
                TotalPrice = 0,
                TotalItems = 0,
                Items = new List<CartItemDto>()
            };
            _mockQueryService.Setup(s => s.GetCartAsync(cartId)).ReturnsAsync(cartDto);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<CartDto>(viewResult.Model);
            Assert.Empty(((CartDto)viewResult.Model).Items);
        }

        [Fact]
        public async Task Index_ReturnsErrorView_WhenExceptionIsThrown()
        {
            // Arrange
            _mockQueryService.Setup(s => s.GetCartAsync(It.IsAny<Guid>())).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
            Assert.Equal("An error occurred while loading the shopping cart.", viewResult.Model);
        }

        [Fact]
        public async Task AddItemToCartAsync_RedirectsToIndex_WhenSuccessful()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var productId = "test-product";
            var quantity = 1;

            _mockService.Setup(s => s.AddItemToCartAsync(cartId, productId, quantity)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AddItemToCartAsync(cartId, productId, quantity);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task AddItemToCartAsync_ReturnsErrorView_WhenQuantityIsInvalid()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var productId = "test-product";
            var quantity = 0; // Invalid quantity

            // Act
            var result = await _controller.AddItemToCartAsync(cartId, productId, quantity);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
            Assert.Equal("Invalid quantity.", viewResult.Model);
        }

        [Fact]
        public async Task AddItemToCartAsync_ReturnsErrorView_WhenExceptionIsThrown()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var productId = "test-product";
            var quantity = 1;

            _mockService.Setup(s => s.AddItemToCartAsync(cartId, productId, quantity)).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.AddItemToCartAsync(cartId, productId, quantity);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
            Assert.Equal("An error occurred while adding the item to the cart.", viewResult.Model);
        }

        [Fact]
        public async Task RemoveItemFromCartAsync_RedirectsToIndex_WhenSuccessful()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var productId = "test-product";

            _mockService.Setup(s => s.RemoveItemFromCartAsync(cartId, productId)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.RemoveItemFromCartAsync(cartId, productId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task RemoveItemFromCartAsync_ReturnsErrorView_WhenProductNotFound()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var productId = "non-existent-product";

            _mockService.Setup(s => s.RemoveItemFromCartAsync(cartId, productId))
                .ThrowsAsync(new KeyNotFoundException("Product not found"));

            // Act
            var result = await _controller.RemoveItemFromCartAsync(cartId, productId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
            Assert.Equal("Product not found.", viewResult.Model);
        }

        [Fact]
        public async Task UpdateItemQuantityAsync_RedirectsToIndex_WhenSuccessful()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var productId = "test-product";
            var quantity = 2;

            _mockService.Setup(s => s.UpdateItemQuantityAsync(cartId, productId, quantity)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateItemQuantityAsync(cartId, productId, quantity);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task UpdateItemQuantityAsync_ReturnsErrorView_WhenQuantityIsInvalid()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var productId = "test-product";
            var quantity = 0; // Invalid quantity

            // Act
            var result = await _controller.UpdateItemQuantityAsync(cartId, productId, quantity);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
            Assert.Equal("Invalid quantity.", viewResult.Model);
        }

        [Fact]
        public async Task UpdateItemQuantityAsync_ReturnsErrorView_WhenExceptionIsThrown()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var productId = "test-product";
            var quantity = 2;

            _mockService.Setup(s => s.UpdateItemQuantityAsync(cartId, productId, quantity)).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.UpdateItemQuantityAsync(cartId, productId, quantity);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
            Assert.Equal("An error occurred while updating the item quantity.", viewResult.Model);
        }
    }
}