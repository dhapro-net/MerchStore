using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MerchStore.WebUI.Models;
using MerchStore.Application.Catalog.Queries;
using MerchStore.Domain.ShoppingCart;
using MerchStore.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Http;
using MerchStore.WebUI.Models.ShoppingCart;

namespace MerchStoreTest.WebUI.Controllers
{
    public class ShoppingCartControllerTest
    {
        private readonly Mock<CookieShoppingCartService> _cartServiceMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ILogger<ShoppingCartController>> _loggerMock;
        private readonly ShoppingCartController _controller;

        public ShoppingCartControllerTest()
        {
            _cartServiceMock = new Mock<CookieShoppingCartService>(null, null, null);
            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<ILogger<ShoppingCartController>>();
            _controller = new ShoppingCartController(_cartServiceMock.Object, _mediatorMock.Object, _loggerMock.Object);
            _controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(
                new DefaultHttpContext(), Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());
        }

        #region Index

        [Fact]
        public void Index_ReturnsViewWithViewModel()
        {
            // Arrange
            var cart = CreateCartWithProducts();
            _cartServiceMock.Setup(s => s.GetOrCreateCart()).Returns(cart);

            // Act
            var result = _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ShoppingCartViewModel>(viewResult.Model);
            Assert.Equal(cart.CartId, model.CartId);
            Assert.Equal(cart.Products.Count, model.Products.Count);
        }

        [Fact]
        public void Index_WhenException_ReturnsErrorView()
        {
            _cartServiceMock.Setup(s => s.GetOrCreateCart()).Throws(new Exception("fail"));
            var result = _controller.Index();
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
            Assert.IsType<ErrorViewModel>(viewResult.Model);
        }

        #endregion

        #region AddProductToCart

        [Fact]
        public async Task AddProductToCart_InvalidProductId_ReturnsRedirectWithError()
        {
            var result = await _controller.AddProductToCart(null, 1);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal("Product ID cannot be null or empty.", _controller.TempData["ErrorMessage"]);
        }

        [Fact]
        public async Task AddProductToCart_InvalidQuantity_ReturnsRedirectWithError()
        {
            var result = await _controller.AddProductToCart(Guid.NewGuid().ToString(), 0);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal("Quantity must be greater than zero.", _controller.TempData["ErrorMessage"]);
        }

        [Fact]
        public async Task AddProductToCart_ProductNotFound_ReturnsRedirectWithError()
        {
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetProductByIdQuery>(), default)).ReturnsAsync((ProductDto)null);
            var result = await _controller.AddProductToCart(Guid.NewGuid().ToString(), 1);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal("The product could not be found.", _controller.TempData["ErrorMessage"]);
        }

        [Fact]
        public async Task AddProductToCart_Success_ReturnsRedirectWithSuccess()
        {
            var productId = Guid.NewGuid().ToString();
            var product = new ProductDto
            {
                Id = Guid.Parse(productId),
                Name = "Test Product",
                Price = new Money(10, "SEK"),
                StockQuantity = 10
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetProductByIdQuery>(), default)).ReturnsAsync(product);

            var cart = CreateCartWithProducts();
            _cartServiceMock.Setup(s => s.GetOrCreateCart()).Returns(cart);
            _cartServiceMock.Setup(s => s.SaveCart(cart));

            var result = await _controller.AddProductToCart(productId, 2);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal("Product added to cart successfully!", _controller.TempData["SuccessMessage"]);
        }

        [Fact]
        public async Task AddProductToCart_Exception_ReturnsErrorView()
        {
            var productId = Guid.NewGuid().ToString();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetProductByIdQuery>(), default)).ThrowsAsync(new Exception("fail"));
            var result = await _controller.AddProductToCart(productId, 1);
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
            Assert.IsType<ErrorViewModel>(viewResult.Model);
        }

        #endregion

        #region ClearCart

        [Fact]
        public void ClearCart_Success_ReturnsRedirectWithSuccess()
        {
            _cartServiceMock.Setup(s => s.ClearCart());
            var result = _controller.ClearCart();
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal("Shopping cart cleared successfully!", _controller.TempData["SuccessMessage"]);
        }

        [Fact]
        public void ClearCart_Exception_ReturnsErrorView()
        {
            _cartServiceMock.Setup(s => s.ClearCart()).Throws(new Exception("fail"));
            var result = _controller.ClearCart();
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
            Assert.IsType<ErrorViewModel>(viewResult.Model);
        }

        #endregion

        #region UpdateQuantity

        [Fact]
        public void UpdateQuantity_InvalidProductId_ReturnsRedirectWithError()
        {
            var result = _controller.UpdateQuantity(null, 1);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal("Product ID cannot be null or empty.", _controller.TempData["ErrorMessage"]);
        }

        [Fact]
        public void UpdateQuantity_InvalidQuantity_ReturnsRedirectWithError()
        {
            var result = _controller.UpdateQuantity(Guid.NewGuid().ToString(), 0);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal("Quantity must be greater than zero.", _controller.TempData["ErrorMessage"]);
        }

        [Fact]
        public void UpdateQuantity_Success_ReturnsRedirectWithSuccess()
        {
            var cart = CreateCartWithProducts();
            _cartServiceMock.Setup(s => s.GetOrCreateCart()).Returns(cart);
            _cartServiceMock.Setup(s => s.SaveCart(cart));
            var result = _controller.UpdateQuantity("prod1", 2);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal("Product quantity updated successfully!", _controller.TempData["SuccessMessage"]);
        }

        [Fact]
        public void UpdateQuantity_Exception_ReturnsErrorView()
        {
            _cartServiceMock.Setup(s => s.GetOrCreateCart()).Throws(new Exception("fail"));
            var result = _controller.UpdateQuantity("prod1", 2);
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
            Assert.IsType<ErrorViewModel>(viewResult.Model);
        }

        #endregion

        #region RemoveProduct

        [Fact]
        public void RemoveProduct_InvalidProductId_ReturnsRedirectWithError()
        {
            var result = _controller.RemoveProduct(null);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal("Product ID cannot be null or empty.", _controller.TempData["ErrorMessage"]);
        }

        [Fact]
        public void RemoveProduct_Success_ReturnsRedirectWithSuccess()
        {
            var cart = CreateCartWithProducts();
            _cartServiceMock.Setup(s => s.GetOrCreateCart()).Returns(cart);
            _cartServiceMock.Setup(s => s.SaveCart(cart));
            var result = _controller.RemoveProduct("prod1");
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal("Product removed from cart successfully!", _controller.TempData["SuccessMessage"]);
        }

        [Fact]
        public void RemoveProduct_Exception_ReturnsErrorView()
        {
            _cartServiceMock.Setup(s => s.GetOrCreateCart()).Throws(new Exception("fail"));
            var result = _controller.RemoveProduct("prod1");
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
            Assert.IsType<ErrorViewModel>(viewResult.Model);
        }

        #endregion

        #region SubmitOrder

        [Fact]
        public void SubmitOrder_ModelInvalid_ReturnsRedirectWithError()
        {
            _controller.ModelState.AddModelError("test", "error");
            var result = _controller.SubmitOrder(new ShoppingCartViewModel());
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal("Please correct the errors in the form.", _controller.TempData["ErrorMessage"]);
        }

        [Fact]
        public void SubmitOrder_EmptyCart_ReturnsRedirectWithError()
        {
            _cartServiceMock.Setup(s => s.GetCart()).Returns((Cart)null);
            var result = _controller.SubmitOrder(new ShoppingCartViewModel());
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal("Your cart is empty. Please add products before submitting an order.", _controller.TempData["ErrorMessage"]);
        }

        [Fact]
        public void SubmitOrder_Success_ReturnsRedirectWithSuccess()
        {
            var cart = CreateCartWithProducts();
            _cartServiceMock.Setup(s => s.GetCart()).Returns(cart);
            _cartServiceMock.Setup(s => s.ClearCart());
            var result = _controller.SubmitOrder(new ShoppingCartViewModel());
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal("Order submitted successfully!", _controller.TempData["SuccessMessage"]);
        }

        [Fact]
        public void SubmitOrder_Exception_ReturnsErrorView()
        {
            _cartServiceMock.Setup(s => s.GetCart()).Throws(new Exception("fail"));
            var result = _controller.SubmitOrder(new ShoppingCartViewModel());
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
            Assert.IsType<ErrorViewModel>(viewResult.Model);
        }

        #endregion

        // Helper method to create a cart with products
        private Cart CreateCartWithProducts()
        {
            var cart = new Cart(Guid.NewGuid(), new List<CartProduct>
            {
                new CartProduct("prod1", "Product 1", new Money(10, "SEK"), 1),
                new CartProduct("prod2", "Product 2", new Money(20, "SEK"), 2)
            }, DateTime.UtcNow, DateTime.UtcNow);
            return cart;
        }
    }
}