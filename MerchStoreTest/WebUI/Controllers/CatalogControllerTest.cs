using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MerchStore.Application.Catalog.Queries;
using MerchStore.Domain.ShoppingCart;
using MerchStore.Domain.ValueObjects;
using MerchStore.WebUI.Controllers;
using MerchStore.WebUI.Models;
using MerchStore.WebUI.Models.Catalog;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace MerchStoreTest.WebUI.Controllers
{
    public class CatalogControllerTest
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ILogger<CatalogController>> _loggerMock;
        private readonly Mock<CookieShoppingCartService> _cartServiceMock;
        private readonly CatalogController _controller;

        public CatalogControllerTest()
        {
            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<ILogger<CatalogController>>();

            // Mock dependencies for CookieShoppingCartService
            var cartLoggerMock = new Mock<ILogger<CookieShoppingCartService>>();
            var cartCartLoggerMock = new Mock<ILogger<Cart>>();
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            _cartServiceMock = new Mock<CookieShoppingCartService>(
                MockBehavior.Strict,
                cartLoggerMock.Object,
                cartCartLoggerMock.Object,
                httpContextAccessorMock.Object
            );

            _controller = new CatalogController(_mediatorMock.Object, _loggerMock.Object, _cartServiceMock.Object);

            // Setup HttpContext for controller
            var httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Setup TempData for controller
            var tempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(
                httpContext,
                Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>()
            );
            _controller.TempData = tempData;
        }

        #region Index

        [Fact]
        public async Task Index_ReturnsViewWithProducts()
        {
            // Arrange
            var products = new List<ProductDto>
            {
                new ProductDto
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Product",
                    Description = "A test product description.",
                    Price = new Money(100, "SEK"),
                    ImageUrl = new Uri("http://test.com/image.jpg"),
                    StockQuantity = 10
                }
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllProductsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(products);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ProductCatalogViewModel>(viewResult.Model);
            Assert.Single(model.FeaturedProducts);
            Assert.Equal("Test Product", model.FeaturedProducts.First().Name);
        }

        [Fact]
        public async Task Index_LogsErrorAndReturnsErrorView_OnException()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllProductsQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
            Assert.Equal("An error occurred while loading products. Please try again later.", _controller.ViewBag.ErrorMessage);
        }

        [Fact]
        public async Task Index_ReturnsEmptyViewModel_WhenNoProducts()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllProductsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ProductDto>());

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ProductCatalogViewModel>(viewResult.Model);
            Assert.Empty(model.FeaturedProducts);
        }

        #endregion

        #region Details

        [Fact]
        public async Task Details_ReturnsViewWithProductDetails()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new ProductDto
            {
                Id = productId,
                Name = "ProductX",
                Description = "DescX",
                ImageUrl = new Uri("http://img.com/x.jpg"),
                Price = new Money(123, "SEK"),
                StockQuantity = 7
            };

            _mediatorMock.Setup(m => m.Send(It.Is<GetProductByIdQuery>(q => q.ProductId == productId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            // Act
            var result = await _controller.Details(productId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ProductDetailsViewModel>(viewResult.Model);
            Assert.Equal(productId, model.Id);
            Assert.Equal("ProductX", model.Name);
        }

        [Fact]
        public async Task Details_LogsErrorAndReturnsErrorView_OnException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Error"));

            // Act
            var result = await _controller.Details(productId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
            Assert.Equal("An error occurred while loading the product. Please try again later.", _controller.ViewBag.ErrorMessage);
        }

        [Fact]
        public async Task Details_ReturnsErrorView_WhenProductIsNull()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ProductDto)null);

            // Act
            var result = await _controller.Details(productId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
            Assert.Equal("An error occurred while loading the product. Please try again later.", _controller.ViewBag.ErrorMessage);
        }

        #endregion

        #region AddProductToCart

        [Fact]
        public async Task AddProductToCart_ReturnsRedirect_WhenProductIdIsNull()
        {
            // Act
            var result = await _controller.AddProductToCart(null, 1);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal("Product ID cannot be null or empty.", _controller.TempData["ErrorMessage"]);
        }

        [Fact]
        public async Task AddProductToCart_ReturnsRedirect_WhenQuantityIsZero()
        {
            // Act
            var result = await _controller.AddProductToCart("some-id", 0);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal("Quantity must be greater than zero.", _controller.TempData["ErrorMessage"]);
        }

        [Fact]
        public async Task AddProductToCart_ReturnsRedirect_WhenProductNotFound()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ProductDto)null);

            // Act
            var result = await _controller.AddProductToCart(Guid.NewGuid().ToString(), 1);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal("The product could not be found.", _controller.TempData["ErrorMessage"]);
        }

        [Fact]
        public async Task AddProductToCart_AddsProductAndRedirects()
        {
            // Arrange
            var productId = Guid.NewGuid().ToString();
            var product = new ProductDto
            {
                Id = Guid.Parse(productId),
                Name = "Product",
                Description = "A test product description.", // Add this line
                ImageUrl = new Uri("http://test.com/image.jpg"), // Add this line
                Price = new Money(50, "SEK"),
                StockQuantity = 10
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var cart = new Cart(
    Guid.NewGuid(),
    new List<CartProduct>(),
    DateTime.UtcNow,
    DateTime.UtcNow
);
            var cartSpy = new Mock<Cart>();
            cartSpy.CallBase = true;
            _cartServiceMock.Setup(s => s.GetOrCreateCart()).Returns(cart);
            _cartServiceMock.Setup(s => s.SaveCart(cart));

            // Act
            var result = await _controller.AddProductToCart(productId, 2);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal("Product added to cart successfully!", _controller.TempData["SuccessMessage"]);
            Assert.Contains(cart.Products, item => item.ProductId == productId && item.ProductName == product.Name && item.UnitPrice == product.Price && item.Quantity == 2); _cartServiceMock.Verify(s => s.SaveCart(cart), Times.Once);
        }

        [Fact]
        public async Task AddProductToCart_LogsErrorAndReturnsErrorView_OnException()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Error"));

            // Act
            var result = await _controller.AddProductToCart(Guid.NewGuid().ToString(), 1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
            var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
            Assert.Equal("An error occurred while adding the product to the cart.", model.Message);
        }

        [Fact]
        public async Task AddProductToCart_ReturnsRedirect_WhenProductIdIsInvalidGuid()
        {
            // Act
            var result = await _controller.AddProductToCart("not-a-guid", 1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
            var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
            Assert.Equal("An error occurred while adding the product to the cart.", model.Message);
        }

        #endregion

        #region Constructor

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenMediatorIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new CatalogController(null, _loggerMock.Object, _cartServiceMock.Object));
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenLoggerIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new CatalogController(_mediatorMock.Object, null, _cartServiceMock.Object));
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenCartServiceIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new CatalogController(_mediatorMock.Object, _loggerMock.Object, null));
        }

        #endregion

        #region ClearCart

        [Fact]
        public void ClearCart_ClearsCartAndRedirects()
        {
            // Arrange
            _cartServiceMock.Setup(s => s.ClearCart());

            // Act
            var result = _controller.ClearCart();

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal("Shopping cart cleared successfully!", _controller.TempData["SuccessMessage"]);
            _cartServiceMock.Verify(s => s.ClearCart(), Times.Once);
        }

        [Fact]
        public void ClearCart_LogsErrorAndReturnsErrorView_OnException()
        {
            // Arrange
            _cartServiceMock.Setup(s => s.ClearCart()).Throws(new Exception("Error"));

            // Act
            var result = _controller.ClearCart();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
            var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
            Assert.Equal("An error occurred while clearing the shopping cart.", model.Message);
        }

        #endregion
    }
}