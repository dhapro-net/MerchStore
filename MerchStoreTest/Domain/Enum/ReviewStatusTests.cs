using System;
using FluentAssertions;
using Xunit;

namespace MerchStore.Domain.Enums;

public class ReviewStatusTests
{
    [Fact]
    public void ReviewStatus_ShouldHaveExpectedValues()
    {
        // Arrange & Act - for enums, these steps are typically combined
        var values = Enum.GetValues<ReviewStatus>();

        // Assert
        values.Should().Contain(ReviewStatus.Pending);
        values.Should().Contain(ReviewStatus.Approved);
        values.Should().Contain(ReviewStatus.Rejected);
        values.Should().HaveCount(3); // Ensures no unexpected values are added
    }

    [Theory]
    [InlineData(ReviewStatus.Pending, "Pending")]
    [InlineData(ReviewStatus.Approved, "Approved")]
    [InlineData(ReviewStatus.Rejected, "Rejected")]
    public void ReviewStatus_ToString_ShouldReturnExpectedString(ReviewStatus status, string expected)
    {
        // Act
        string result = status.ToString();

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void ReviewStatus_DefaultValue_ShouldBePending()
    {
        // Arrange
        ReviewStatus defaultValue = default;

        // Act & Assert
        defaultValue.Should().Be(ReviewStatus.Pending);
    }
    //add mote test cases to check the enum values and their string representations
    [Theory]
    [InlineData("Pending", ReviewStatus.Pending)]
    [InlineData("Approved", ReviewStatus.Approved)]
    [InlineData("Rejected", ReviewStatus.Rejected)]
    public void ReviewStatus_Parse_ShouldReturnCorrectEnum(string input, ReviewStatus expected)
    {
        Enum.Parse<ReviewStatus>(input).Should().Be(expected);
    }

    [Fact]
    public void ReviewStatus_Parse_InvalidValue_ShouldThrow()
    {
        Action act = () => Enum.Parse<ReviewStatus>("InvalidStatus");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ReviewStatus_TryParse_InvalidValue_ShouldReturnFalse()
    {
        bool success = Enum.TryParse("InvalidStatus", out ReviewStatus result);
        success.Should().BeFalse();
    }

    [Theory]
    [InlineData(0, ReviewStatus.Pending)]
    [InlineData(1, ReviewStatus.Approved)]
    [InlineData(2, ReviewStatus.Rejected)]
    public void ReviewStatus_ShouldHaveExpectedIntegerValues(int expectedInt, ReviewStatus value)
    {
        ((int)value).Should().Be(expectedInt);
    }

}