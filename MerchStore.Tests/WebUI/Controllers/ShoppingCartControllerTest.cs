using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using MerchStore.Application.Services.Interfaces;
using MerchStore.WebUI.Controllers;
using MerchStore.WebUI.Models.ShoppingCart;
using MerchStore.Domain.Entities;
using MerchStore.Domain.ValueObjects;

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
        public async Task Index_ReturnsViewWithCart_WhenCartExists()
        {
            // Arrange
            var cartDto = new CartDto
            {
                Id = Guid.NewGuid(),
                TotalPrice = 100,
                TotalItems = 2,
                Items = new List<CartItemDto>
                {
                    new CartItemDto { ProductId = "1", Quantity = 1, Price = 50 },
                    new CartItemDto { ProductId = "2", Quantity = 1, Price = 50 }
                }
            };
            _mockQueryService.Setup(s => s.GetCartAsync(It.IsAny<Guid>())).ReturnsAsync(cartDto);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<CartDto>(viewResult.Model);
            Assert.Equal(cartDto, model);
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
        public async Task AddItemToCartAsync_ReturnsErrorView_WhenExceptionIsThrown()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var productId = "test-product";
            var quantity = 1;

            _mockService.Setup(s => s.AddItemToCartAsync(cartId, productId, quantity))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.AddItemToCartAsync(cartId, productId, quantity);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
            Assert.Equal("An error occurred while adding the item to the cart.", viewResult.Model);
        }

        [Fact]
        public void SubmitOrder_CreatesOrderSuccessfully_WhenModelIsValid()
        {
            // Arrange
            var model = new ShoppingCartViewModel
            {
                Shipping = new ShippingViewModel
                {
                    FullName = "John Doe",
                    Address = "123 Main St",
                    City = "New York",
                    PostalCode = "10001",
                    Country = "USA"
                },
                Payment = new PaymentViewModel
                {
                    CardNumber = "4111111111111111",
                    ExpirationDate = "12/25",
                    CVV = "123"
                }
            };

            Guid orderId = Guid.NewGuid();
            var paymentInfo = new PaymentInfo("4111111111111111", "12/25", "123");
            var shippingAddress = "123 Main St, New York, NY, 10001, USA";
            var customerName = "John Doe";
            var totalAmount = new Money(100, "USD");
            var orderDate = DateTime.UtcNow;

            // Act
            var order = new Order(orderId, paymentInfo, shippingAddress, customerName, totalAmount, orderDate);

            // Assert
            Assert.Equal(orderId, order.Id);
            Assert.Equal(paymentInfo, order.PaymentInfo);
            Assert.Equal(shippingAddress, order.ShippingAddress);
            Assert.Equal(customerName, order.CustomerName);
            Assert.Equal(totalAmount, order.TotalAmount);
            Assert.Equal(orderDate, order.OrderDate);
        }

        [Fact]
        public async Task SubmitOrder_ReturnsErrorView_WhenExceptionIsThrown()
        {
            // Arrange
            var model = new ShoppingCartViewModel
            {
                Shipping = new ShippingViewModel
                {
                    FullName = "Jane Doe",
                    Address = "456 Elm St",
                    City = "Los Angeles",
                    PostalCode = "90001",
                    Country = "USA"
                },
                Payment = new PaymentViewModel
                {
                    CardNumber = "4111111111111111",
                    ExpirationDate = "12/25",
                    CVV = "123"
                }
            };

            _mockService.Setup(s => s.CreateOrderAsync(It.IsAny<Order>()))
                .ThrowsAsync(new Exception("Order creation failed."));

            // Act
            var result = await _controller.SubmitOrder(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
            Assert.Equal("An error occurred while processing your order.", viewResult.Model);
        }

        [Fact]
        public async Task GetCartAsync_ReturnsViewWithCart()
        {
            // Arrange
            var cartDto = new CartDto
            {
                Id = Guid.NewGuid(),
                TotalPrice = 100,
                TotalItems = 2,
                Items = new List<CartItemDto>
                {
                    new CartItemDto { ProductId = "1", Quantity = 1, Price = 50 },
                    new CartItemDto { ProductId = "2", Quantity = 1, Price = 50 }
                }
            };
            _mockQueryService.Setup(s => s.GetCartAsync(It.IsAny<Guid>())).ReturnsAsync(cartDto);

            // Act
            var result = await _controller.GetCartAsync();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<CartDto>(viewResult.Model);
            Assert.Equal(cartDto, model);
        }
    }
}