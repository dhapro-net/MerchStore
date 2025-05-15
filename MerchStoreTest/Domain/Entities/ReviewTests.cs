using System;
using FluentAssertions;
using MerchStore.Domain.Entities;
using MerchStore.Domain.Enums;
using Xunit;

namespace MerchStore.Domain.Tests.Entities;

public class ReviewTests
{
    private readonly Guid _validId = Guid.NewGuid();
    private readonly Guid _validProductId = Guid.NewGuid();
    private const string _validCustomerName = "John Doe";
    private const string _validTitle = "Great Product";
    private const string _validContent = "This product exceeded my expectations!";
    private const int _validRating = 5;
    private readonly DateTime _validCreatedAt = DateTime.UtcNow;
    private const ReviewStatus _validStatus = ReviewStatus.Approved;

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange & Act
        var review = new Review(
            _validId,
            _validProductId,
            _validCustomerName,
            _validTitle,
            _validContent,
            _validRating,
            _validCreatedAt,
            _validStatus);

        // Assert
        review.Id.Should().Be(_validId);
        review.ProductId.Should().Be(_validProductId);
        review.CustomerName.Should().Be(_validCustomerName);
        review.Title.Should().Be(_validTitle);
        review.Content.Should().Be(_validContent);
        review.Rating.Should().Be(_validRating);
        review.CreatedAt.Should().Be(_validCreatedAt);
        review.Status.Should().Be(_validStatus);
    }

    [Fact]
    public void Constructor_WithEmptyId_ShouldThrowArgumentException()
    {
        // Arrange & Act
        Action act = () => new Review(
            Guid.Empty,
            _validProductId,
            _validCustomerName,
            _validTitle,
            _validContent,
            _validRating,
            _validCreatedAt,
            _validStatus);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*entity ID cannot be*");
    }

    [Fact]
    public void Constructor_WithEmptyProductId_ShouldThrowArgumentException()
    {
        // Arrange & Act
        Action act = () => new Review(
            _validId,
            Guid.Empty,
            _validCustomerName,
            _validTitle,
            _validContent,
            _validRating,
            _validCreatedAt,
            _validStatus);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Product ID cannot be empty*")
            .And.ParamName.Should().Be("productId");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidCustomerName_ShouldThrowArgumentException(string? invalidName)
    {
        // Arrange & Act
        Action act = () => new Review(
            _validId,
            _validProductId,
            invalidName!,
            _validTitle,
            _validContent,
            _validRating,
            _validCreatedAt,
            _validStatus);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Customer name cannot be empty*")
            .And.ParamName.Should().Be("customerName");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidTitle_ShouldThrowArgumentException(string? invalidTitle)
    {
        // Arrange & Act
        Action act = () => new Review(
            _validId,
            _validProductId,
            _validCustomerName,
            invalidTitle!,
            _validContent,
            _validRating,
            _validCreatedAt,
            _validStatus);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Review title cannot be empty*")
            .And.ParamName.Should().Be("title");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidContent_ShouldThrowArgumentException(string? invalidContent)
    {
        // Arrange & Act
        Action act = () => new Review(
            _validId,
            _validProductId,
            _validCustomerName,
            _validTitle,
            invalidContent!,
            _validRating,
            _validCreatedAt,
            _validStatus);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Review content cannot be empty*")
            .And.ParamName.Should().Be("content");
    }

    [Theory]
    [InlineData(0)]  // Too low
    [InlineData(-1)] // Negative
    [InlineData(6)]  // Too high
    [InlineData(int.MinValue)]
    [InlineData(int.MaxValue)]
    public void Constructor_WithInvalidRating_ShouldThrowArgumentOutOfRangeException(int invalidRating)
    {
        // Arrange & Act
        Action act = () => new Review(
            _validId,
            _validProductId,
            _validCustomerName,
            _validTitle,
            _validContent,
            invalidRating,
            _validCreatedAt,
            _validStatus);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Rating must be between 1 and 5*")
            .And.ParamName.Should().Be("rating");
    }

    [Theory]
    [InlineData(1)] // Minimum valid
    [InlineData(3)] // Middle value
    [InlineData(5)] // Maximum valid
    public void Constructor_WithValidRating_ShouldNotThrow(int validRating)
    {
        // Arrange & Act
        Action act = () => new Review(
            _validId,
            _validProductId,
            _validCustomerName,
            _validTitle,
            _validContent,
            validRating,
            _validCreatedAt,
            _validStatus);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Equals_WithSameId_ShouldBeTrue()
    {
        // Arrange
        var review1 = new Review(
            _validId,
            _validProductId,
            _validCustomerName,
            _validTitle,
            _validContent,
            _validRating,
            _validCreatedAt,
            _validStatus);

        var review2 = new Review(
            _validId, // Same ID
            Guid.NewGuid(), // Different product
            "Different Name",
            "Different Title",
            "Different Content",
            4, // Different rating
            DateTime.UtcNow.AddDays(-1), // Different date
            ReviewStatus.Pending); // Different status

        // Act & Assert
        review1.Equals(review2).Should().BeTrue();
        (review1 == review2).Should().BeTrue();
        (review1 != review2).Should().BeFalse();
        review1.GetHashCode().Should().Be(review2.GetHashCode());
    }

    [Fact]
    public void Equals_WithDifferentId_ShouldBeFalse()
    {
        // Arrange
        var review1 = new Review(
            _validId,
            _validProductId,
            _validCustomerName,
            _validTitle,
            _validContent,
            _validRating,
            _validCreatedAt,
            _validStatus);

        var review2 = new Review(
            Guid.NewGuid(), // Different ID
            _validProductId, // Same product
            _validCustomerName, // Same name
            _validTitle, // Same title
            _validContent, // Same content
            _validRating, // Same rating
            _validCreatedAt, // Same date
            _validStatus); // Same status

        // Act & Assert
        review1.Equals(review2).Should().BeFalse();
        (review1 == review2).Should().BeFalse();
        (review1 != review2).Should().BeTrue();
    }
    //more test cases
    [Fact]
    public void Equals_WithNull_ShouldReturnFalse()
    {
        var review = new Review(
            _validId,
            _validProductId,
            _validCustomerName,
            _validTitle,
            _validContent,
            _validRating,
            _validCreatedAt,
            _validStatus);

        review.Equals(null).Should().BeFalse();
    }

    [Fact]
    public void Equals_WithDifferentType_ShouldReturnFalse()
    {
        var review = new Review(
            _validId,
            _validProductId,
            _validCustomerName,
            _validTitle,
            _validContent,
            _validRating,
            _validCreatedAt,
            _validStatus);

        review.Equals("not a review").Should().BeFalse();
    }
    [Fact]
    public void Constructor_ShouldPreserveCreatedAtTimestamp()
    {
        var timestamp = DateTime.UtcNow;

        var review = new Review(
            _validId,
            _validProductId,
            _validCustomerName,
            _validTitle,
            _validContent,
            _validRating,
            timestamp,
            _validStatus);

        review.CreatedAt.Should().Be(timestamp);
    }
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void Constructor_WithInvalidTitle_ShouldThrow(string title)
    {
        Action act = () => new Review(
            _validId,
            _validProductId,
            _validCustomerName,
            title,
            _validContent,
            _validRating,
            _validCreatedAt,
            _validStatus);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Review title cannot be empty*")
            .And.ParamName.Should().Be("title");
    }

}