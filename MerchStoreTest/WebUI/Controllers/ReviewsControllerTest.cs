using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MerchStore.WebUI.Controllers;
using MerchStore.Application.Services.Interfaces;
using MerchStore.WebUI.Models;
using Microsoft.AspNetCore.Http;
using MerchStore.Domain.Entities;
using System.Linq;

namespace MerchStoreTest.WebUI.Controllers
{
    public class ReviewsControllerTest
    {
        private readonly Mock<IReviewService> _reviewServiceMock;
        private readonly Mock<ICatalogService> _catalogServiceMock;
        private readonly ReviewsController _controller;

        public ReviewsControllerTest()
        {
            _reviewServiceMock = new Mock<IReviewService>();
            _catalogServiceMock = new Mock<ICatalogService>();
            _controller = new ReviewsController(_reviewServiceMock.Object, _catalogServiceMock.Object);
            _controller.TempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(
                new DefaultHttpContext(), Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>());
        }

        #region Index




        [Fact]
        public async Task Index_WhenException_ReturnsErrorView()
        {
            _catalogServiceMock.Setup(s => s.GetAllProductsAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("fail"));
            var result = await _controller.Index(CancellationToken.None);
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
            Assert.Contains("Error fetching reviews", _controller.TempData["ErrorMessage"] as string);
        }

        #endregion

        #region Product


        [Fact]
        public async Task Product_ProductNotFound_ReturnsNotFound()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _catalogServiceMock
                .Setup(s => s.GetProductByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product)null); // or (ProductDto)null                .ReturnsAsync((ProductViewModel)null);

            // Act
            var result = await _controller.Product(productId, CancellationToken.None);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Product_WhenException_ReturnsErrorView()
        {
            var productId = Guid.NewGuid();
            _catalogServiceMock.Setup(s => s.GetProductByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("fail"));

            var result = await _controller.Product(productId, CancellationToken.None);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
            Assert.Contains("Error fetching product reviews", _controller.TempData["ErrorMessage"] as string);
        }

        #endregion
    }
}

// Minimal stubs for view models to make the tests compile
public class ProductViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}
public class ReviewViewModel { }
public class ProductReviewsViewModel
{
    public List<ProductViewModel> Products { get; set; } = new();
    public Dictionary<Guid, List<ReviewViewModel>> ProductReviews { get; set; } = new();
    public Dictionary<Guid, double> AverageRatings { get; set; } = new();
    public Dictionary<Guid, int> ReviewCounts { get; set; } = new();
}
public class ProductReviewViewModel
{
    public ProductViewModel Product { get; set; }
    public List<ReviewViewModel> Reviews { get; set; }
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
}
