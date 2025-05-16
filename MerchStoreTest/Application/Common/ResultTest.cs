using System;
using MerchStore.Application.Common;
using Xunit;

namespace MerchStoreTest.Application.Common;

public class ResultTest
{
    [Fact]
    public void Success_ShouldSetIsSuccessTrue_AndErrorEmpty()
    {
        var result = Result.Success();
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Equal(string.Empty, result.Error);
    }

    [Fact]
    public void Failure_ShouldSetIsSuccessFalse_AndErrorSet()
    {
        var result = Result.Failure("fail");
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal("fail", result.Error);
    }

    [Fact]
    public void SuccessT_ShouldSetValue_AndIsSuccessTrue()
    {
        var result = Result.Success(42);
        Assert.True(result.IsSuccess);
        Assert.Equal(42, ((Result<int>)result).Value);
    }

    [Fact]
    public void FailureT_ShouldSetIsSuccessFalse_AndErrorSet()
    {
        var result = Result.Failure<int>("fail");
        Assert.False(result.IsSuccess);
        Assert.Equal("fail", result.Error);
    }

    [Fact]
    public void FailureT_AccessingValue_Throws()
    {
        var result = Result.Failure<int>("fail");
        Assert.Throws<InvalidOperationException>(() => { var _ = ((Result<int>)result).Value; });
    }

    [Fact]
    public void Success_WithError_Throws()
    {
        var result = Result.Success();
        Assert.True(result.IsSuccess);
        Assert.Equal(string.Empty, result.Error);
    }

[Fact]
public void Failure_WithoutError_Throws()
{
    Assert.Throws<InvalidOperationException>(() => Result.Failure(""));
}
}