using FluentAssertions;
using MerchStore.Domain.Entities;
using MerchStore.Domain.Enums;
using MerchStore.Domain.Interfaces;
using MerchStore.Domain.ValueObjects;
using Moq;

namespace MerchStore.Domain.Tests.Interfaces;

public class ReviewRepositoryTests
{
    private readonly Guid _productId = Guid.NewGuid();
    private readonly Mock<IReviewRepository> _repositoryMock;

    public ReviewRepositoryTests()
    {
        _repositoryMock = new Mock<IReviewRepository>();
    }

    [Fact]
    public async Task GetProductReviewsAsync_ShouldReturnReviewsAndStats()
    {
        // Arrange
        var reviews = new List<Review>
        {
            new Review(
                Guid.NewGuid(),
                _productId,
                "Customer 1",
                "Great Product",
                "I love it!",
                5,
                DateTime.UtcNow.AddDays(-2),
                ReviewStatus.Approved),
            new Review(
                Guid.NewGuid(),
                _productId,
                "Customer 2",
                "Good Product",
                "Pretty good.",
                4,
                DateTime.UtcNow.AddDays(-1),
                ReviewStatus.Approved)
        };

        var stats = new ReviewStats(_productId, 4.5, 2);

        _repositoryMock.Setup(r => r.GetProductReviewsAsync(_productId))
            .ReturnsAsync((reviews, stats));

        // Act
        var result = await _repositoryMock.Object.GetProductReviewsAsync(_productId);

        // Assert
        result.Reviews.Should().BeEquivalentTo(reviews);
        result.Stats.Should().Be(stats);
    }

    [Fact]
    public async Task GetProductReviewsAsync_WithNoReviews_ShouldReturnEmptyListAndZeroStats()
    {
        // Arrange
        var emptyReviews = Enumerable.Empty<Review>();
        var zeroStats = new ReviewStats(_productId, 0, 0);

        _repositoryMock.Setup(r => r.GetProductReviewsAsync(_productId))
            .ReturnsAsync((emptyReviews, zeroStats));

        // Act
        var result = await _repositoryMock.Object.GetProductReviewsAsync(_productId);

        // Assert
        result.Reviews.Should().BeEmpty();
        result.Stats.AverageRating.Should().Be(0);
        result.Stats.ReviewCount.Should().Be(0);
    }
    //more tests cases
    [Fact]
    public async Task GetProductReviewsAsync_WhenRepositoryThrows_ShouldThrowException()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetProductReviewsAsync(_productId))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        Func<Task> act = async () => await _repositoryMock.Object.GetProductReviewsAsync(_productId);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("*Database error*");
    }

}