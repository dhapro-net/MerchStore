using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MerchStore.Application.Services.Implementations;
using MerchStore.Domain.Entities;
using MerchStore.Domain.Interfaces;
using MerchStore.Domain.ValueObjects;
using Moq;
using Xunit;
namespace MerchStoreTest.Application.Services.Implementations;

public class ReviewServiceTest
{
    private readonly Mock<IReviewRepository> _repoMock;
    private readonly ReviewService _service;

    public ReviewServiceTest()
    {
        _repoMock = new Mock<IReviewRepository>();
        _service = new ReviewService(_repoMock.Object);
    }

    [Fact]
    public async Task GetReviewsByProductIdAsync_ReturnsReviews()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var reviews = new List<Review>
        {
            new Review(Guid.NewGuid(), productId, "Alice", "Title", "Content", 5, DateTime.UtcNow, MerchStore.Domain.Enums.ReviewStatus.Approved)
        };
        var stats = new ReviewStats(productId, 5, 1);
        _repoMock.Setup(r => r.GetProductReviewsAsync(productId))
            .ReturnsAsync((reviews, stats));

        // Act
        var result = await _service.GetReviewsByProductIdAsync(productId);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Alice", ((List<Review>)result)[0].CustomerName);
    }

    [Fact]
    public async Task GetAverageRatingForProductAsync_ReturnsAverageRating()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var reviews = new List<Review>();
        var stats = new ReviewStats(productId, 4.2, 3);
        _repoMock.Setup(r => r.GetProductReviewsAsync(productId))
            .ReturnsAsync((reviews, stats));

        // Act
        var result = await _service.GetAverageRatingForProductAsync(productId);

        // Assert
        Assert.Equal(4.2, result);
    }

    [Fact]
    public async Task GetReviewCountForProductAsync_ReturnsReviewCount()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var reviews = new List<Review>();
        var stats = new ReviewStats(productId, 3.5, 7);
        _repoMock.Setup(r => r.GetProductReviewsAsync(productId))
            .ReturnsAsync((reviews, stats));

        // Act
        var result = await _service.GetReviewCountForProductAsync(productId);

        // Assert
        Assert.Equal(7, result);
    }

    [Fact]
    public async Task AddReviewAsync_ThrowsNotImplementedException()
    {
        // Arrange
        var review = new Review(Guid.NewGuid(), Guid.NewGuid(), "Bob", "Title", "Content", 5, DateTime.UtcNow, MerchStore.Domain.Enums.ReviewStatus.Approved);

        // Act & Assert
        await Assert.ThrowsAsync<NotImplementedException>(() => _service.AddReviewAsync(review));
    }
}