using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MerchStore.Application.Services.Interfaces;
using MerchStore.Domain.Entities;
using MerchStore.WebUI.Controllers;
using MerchStore.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace MerchStore.Tests.WebUI.Controllers
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
        }

        #region Index

        [Fact]
        public async Task Index_ReturnsViewWithProductReviews()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var products = new List<Product>
            {
                new Product { Id = productId, Name = "Test Product" }
            };
            var reviews = new List<Review>
            {
                new Review { Id = Guid.NewGuid(), ProductId = productId, Rating = 5, Comment = "Great!" }
            };

            _catalogServiceMock.Setup(s => s.GetAllProductsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(products);
            _reviewServiceMock.Setup(s => s.GetReviewsByProductIdAsync(productId))
                .ReturnsAsync(reviews);
            _reviewServiceMock.Setup(s => s.GetAverageRatingForProductAsync(productId))
                .ReturnsAsync(5.0);
            _reviewServiceMock.Setup(s => s.GetReviewCountForProductAsync(productId))
                .ReturnsAsync(1);

            // Act
            var result = await _controller.Index(CancellationToken.None);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ProductReviewsViewModel>(viewResult.Model);
            Assert.Single(model.Products);
            Assert.Equal(productId, model.Products[0].Id);
            Assert.Single(model.ProductReviews[productId]);
            Assert.Equal(5.0, model.AverageRatings[productId]);
            Assert.Equal(1, model.ReviewCounts[productId]);
        }

        [Fact]
        public async Task Index_ReturnsViewWithEmptyProducts_WhenNoProductsExist()
        {
            // Arrange
            _catalogServiceMock.Setup(s => s.GetAllProductsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Product>());

            // Act
            var result = await _controller.Index(CancellationToken.None);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ProductReviewsViewModel>(viewResult.Model);
            Assert.Empty(model.Products);
            Assert.Empty(model.ProductReviews);
            Assert.Empty(model.AverageRatings);
            Assert.Empty(model.ReviewCounts);
        }

        [Fact]
        public async Task Index_HandlesNullProductListGracefully()
        {
            // Arrange
            _catalogServiceMock.Setup(s => s.GetAllProductsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync((IEnumerable<Product>)null);

            // Act
            var result = await _controller.Index(CancellationToken.None);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
            Assert.Contains("Error fetching reviews", _controller.TempData["ErrorMessage"].ToString());
        }

        [Fact]
        public async Task Index_ReturnsErrorView_OnException()
        {
            // Arrange
            _catalogServiceMock.Setup(s => s.GetAllProductsAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _controller.Index(CancellationToken.None);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
            Assert.Contains("Error fetching reviews", _controller.TempData["ErrorMessage"].ToString());
        }

        #endregion

        #region Product

        [Fact]
        public async Task Product_ReturnsViewWithProductReviewViewModel()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product { Id = productId, Name = "Test Product" };
            var reviews = new List<Review>
            {
                new Review { Id = Guid.NewGuid(), ProductId = productId, Rating = 4, Comment = "Nice!" }
            };

            _catalogServiceMock.Setup(s => s.GetProductByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);
            _reviewServiceMock.Setup(s => s.GetReviewsByProductIdAsync(productId))
                .ReturnsAsync(reviews);
            _reviewServiceMock.Setup(s => s.GetAverageRatingForProductAsync(productId))
                .ReturnsAsync(4.0);
            _reviewServiceMock.Setup(s => s.GetReviewCountForProductAsync(productId))
                .ReturnsAsync(1);

            // Act
            var result = await _controller.Product(productId, CancellationToken.None);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ProductReviewViewModel>(viewResult.Model);
            Assert.Equal(productId, model.Product.Id);
            Assert.Single(model.Reviews);
            Assert.Equal(4.0, model.AverageRating);
            Assert.Equal(1, model.ReviewCount);
        }

        [Fact]
        public async Task Product_ReturnsViewWithNoReviews_WhenNoReviewsExist()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product { Id = productId, Name = "Test Product" };

            _catalogServiceMock.Setup(s => s.GetProductByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);
            _reviewServiceMock.Setup(s => s.GetReviewsByProductIdAsync(productId))
                .ReturnsAsync(new List<Review>());
            _reviewServiceMock.Setup(s => s.GetAverageRatingForProductAsync(productId))
                .ReturnsAsync(0.0);
            _reviewServiceMock.Setup(s => s.GetReviewCountForProductAsync(productId))
                .ReturnsAsync(0);

            // Act
            var result = await _controller.Product(productId, CancellationToken.None);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ProductReviewViewModel>(viewResult.Model);
            Assert.Equal(productId, model.Product.Id);
            Assert.Empty(model.Reviews);
            Assert.Equal(0.0, model.AverageRating);
            Assert.Equal(0, model.ReviewCount);
        }

        [Fact]
        public async Task Product_ReturnsNotFound_WhenProductIsNull()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _catalogServiceMock.Setup(s => s.GetProductByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product)null);

            // Act
            var result = await _controller.Product(productId, CancellationToken.None);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Product_ReturnsErrorView_OnException()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _catalogServiceMock.Setup(s => s.GetProductByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Service error"));

            // Act
            var result = await _controller.Product(productId, CancellationToken.None);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
            Assert.Contains("Error fetching product reviews", _controller.TempData["ErrorMessage"].ToString());
        }

        [Fact]
        public async Task Product_ReturnsErrorView_WhenReviewServiceThrows()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product { Id = productId, Name = "Test Product" };

            _catalogServiceMock.Setup(s => s.GetProductByIdAsync(productId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);
            _reviewServiceMock.Setup(s => s.GetReviewsByProductIdAsync(productId))
                .ThrowsAsync(new Exception("Review service error"));

            // Act
            var result = await _controller.Product(productId, CancellationToken.None);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error", viewResult.ViewName);
            Assert.Contains("Error fetching product reviews", _controller.TempData["ErrorMessage"].ToString());
        }

        #endregion
    }
}