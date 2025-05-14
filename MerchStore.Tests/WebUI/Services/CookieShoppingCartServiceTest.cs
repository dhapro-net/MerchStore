using System;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using MerchStore.Domain.ShoppingCart;
using MerchStore.WebUI.Services;

namespace MerchStore.Tests.WebUI.Services
{
    public class CookieShoppingCartServiceTest
    {
        private readonly Mock<ILogger<CookieShoppingCartService>> _loggerMock;
        private readonly Mock<ILogger<Cart>> _cartLoggerMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly Mock<HttpContext> _httpContextMock;
        private readonly Mock<HttpRequest> _httpRequestMock;
        private readonly Mock<HttpResponse> _httpResponseMock;
        private readonly Mock<IResponseCookies> _responseCookiesMock;
        private readonly Mock<IRequestCookieCollection> _requestCookiesMock;

        private readonly CookieShoppingCartService _service;

        public CookieShoppingCartServiceTest()
        {
            _loggerMock = new Mock<ILogger<CookieShoppingCartService>>();
            _cartLoggerMock = new Mock<ILogger<Cart>>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _httpContextMock = new Mock<HttpContext>();
            _httpRequestMock = new Mock<HttpRequest>();
            _httpResponseMock = new Mock<HttpResponse>();
            _responseCookiesMock = new Mock<IResponseCookies>();
            _requestCookiesMock = new Mock<IRequestCookieCollection>();

            _httpContextMock.SetupGet(c => c.Request).Returns(_httpRequestMock.Object);
            _httpContextMock.SetupGet(c => c.Response).Returns(_httpResponseMock.Object);
            _httpRequestMock.SetupGet(r => r.Cookies).Returns(_requestCookiesMock.Object);
            _httpResponseMock.SetupGet(r => r.Cookies).Returns(_responseCookiesMock.Object);
            _httpContextAccessorMock.SetupGet(a => a.HttpContext).Returns(_httpContextMock.Object);

            _service = new CookieShoppingCartService(
                _loggerMock.Object,
                _cartLoggerMock.Object,
                _httpContextAccessorMock.Object
            );
        }

        #region GetCart

        [Fact]
        public void GetCart_ReturnsCart_WhenCookieExistsAndValid()
        {
            // Arrange
            var cart = Cart.Create(Guid.NewGuid(), _cartLoggerMock.Object);
            var serialized = JsonSerializer.Serialize(cart, new JsonSerializerOptions { PropertyNameCaseInsensitive = true, WriteIndented = false });
            _requestCookiesMock.Setup(c => c["ShoppingCart"]).Returns(serialized);

            // Act
            var result = _service.GetCart();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cart.Id, result.Id);
        }

        [Fact]
        public void GetCart_ReturnsNull_WhenHttpContextIsNull()
        {
            // Arrange
            _httpContextAccessorMock.SetupGet(a => a.HttpContext).Returns((HttpContext)null);

            // Act
            var result = new CookieShoppingCartService(_loggerMock.Object, _cartLoggerMock.Object, _httpContextAccessorMock.Object).GetCart();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetCart_ReturnsNull_WhenCookieIsMissing()
        {
            // Arrange
            _requestCookiesMock.Setup(c => c["ShoppingCart"]).Returns((string)null);

            // Act
            var result = _service.GetCart();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetCart_ReturnsNull_WhenCookieIsEmpty()
        {
            // Arrange
            _requestCookiesMock.Setup(c => c["ShoppingCart"]).Returns(string.Empty);

            // Act
            var result = _service.GetCart();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetCart_ReturnsNull_WhenDeserializationFails()
        {
            // Arrange
            _requestCookiesMock.Setup(c => c["ShoppingCart"]).Returns("not-a-json");

            // Act
            var result = _service.GetCart();

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region SaveCart

        [Fact]
        public void SaveCart_SavesCartToCookie_WhenValid()
        {
            // Arrange
            var cart = Cart.Create(Guid.NewGuid(), _cartLoggerMock.Object);

            // Act
            _service.SaveCart(cart);

            // Assert
            _responseCookiesMock.Verify(c => c.Append(
                "ShoppingCart",
                It.IsAny<string>(),
                It.Is<CookieOptions>(o => o.HttpOnly && o.Secure && o.SameSite == SameSiteMode.Lax && o.Expires.HasValue)
            ), Times.Once);
        }

        [Fact]
        public void SaveCart_ThrowsArgumentNullException_WhenCartIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _service.SaveCart(null));
        }

        [Fact]
        public void SaveCart_DoesNothing_WhenHttpContextIsNull()
        {
            // Arrange
            _httpContextAccessorMock.SetupGet(a => a.HttpContext).Returns((HttpContext)null);
            var service = new CookieShoppingCartService(_loggerMock.Object, _cartLoggerMock.Object, _httpContextAccessorMock.Object);
            var cart = Cart.Create(Guid.NewGuid(), _cartLoggerMock.Object);

            // Act
            service.SaveCart(cart);

            // Assert
            // No exception thrown, no cookie set
            _responseCookiesMock.Verify(c => c.Append(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CookieOptions>()), Times.Never);
        }

        #endregion

        #region ClearCart

        [Fact]
        public void ClearCart_DeletesCookie_WhenHttpContextExists()
        {
            // Act
            _service.ClearCart();

            // Assert
            _responseCookiesMock.Verify(c => c.Delete("ShoppingCart"), Times.Once);
        }

        [Fact]
        public void ClearCart_DoesNothing_WhenHttpContextIsNull()
        {
            // Arrange
            _httpContextAccessorMock.SetupGet(a => a.HttpContext).Returns((HttpContext)null);
            var service = new CookieShoppingCartService(_loggerMock.Object, _cartLoggerMock.Object, _httpContextAccessorMock.Object);

            // Act
            service.ClearCart();

            // Assert
            _responseCookiesMock.Verify(c => c.Delete(It.IsAny<string>()), Times.Never);
        }

        #endregion

        #region GetOrCreateCart

        [Fact]
        public void GetOrCreateCart_ReturnsExistingCart_WhenCartExists()
        {
            // Arrange
            var cart = Cart.Create(Guid.NewGuid(), _cartLoggerMock.Object);
            var serialized = JsonSerializer.Serialize(cart, new JsonSerializerOptions { PropertyNameCaseInsensitive = true, WriteIndented = false });
            _requestCookiesMock.Setup(c => c["ShoppingCart"]).Returns(serialized);

            // Act
            var result = _service.GetOrCreateCart();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cart.Id, result.Id);
        }

        [Fact]
        public void GetOrCreateCart_CreatesAndSavesCart_WhenCartDoesNotExist()
        {
            // Arrange
            _requestCookiesMock.Setup(c => c["ShoppingCart"]).Returns((string)null);

            // Act
            var result = _service.GetOrCreateCart();

            // Assert
            Assert.NotNull(result);
            _responseCookiesMock.Verify(c => c.Append(
                "ShoppingCart",
                It.IsAny<string>(),
                It.IsAny<CookieOptions>()
            ), Times.Once);
        }

        #endregion
    }
}