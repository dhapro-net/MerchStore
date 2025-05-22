using System;
using MerchStore.Domain.ValueObjects;
using Xunit;
namespace MerchStoreTest.Domain.ValueObjects;

public class PaymentInfoTest
{



    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("1234")]
    [InlineData("123456781234567a")]
    [InlineData("123456781234567")]
    [InlineData("12345678123456789")]
    public void Constructor_Throws_WhenCardNumberInvalid(string badCard)
    {
        var expiration = DateTime.UtcNow.AddYears(1).ToString("MM/yy");
        var cvv = "123";
        Assert.Throws<ArgumentException>(() => new PaymentInfo(badCard, expiration, cvv));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("13/25")] // Invalid month
    [InlineData("00/25")] // Invalid month
    [InlineData("12-25")] // Wrong format
    [InlineData("12/2025")] // Wrong format
    [InlineData("12/99")] // Far future, but let's allow it if logic allows
    [InlineData("01/00")] // Past
    public void Constructor_Throws_WhenExpirationDateInvalid(string badExp)
    {
        var cardNumber = "1234567812345678";
        var cvv = "123";
        Assert.Throws<ArgumentException>(() => new PaymentInfo(cardNumber, badExp, cvv));
    }

    [Fact]
    public void Constructor_Throws_WhenExpirationDateInPast()
    {
        var cardNumber = "1234567812345678";
        var cvv = "123";
        var past = DateTime.UtcNow.AddYears(-1).ToString("MM/yy");
        Assert.Throws<ArgumentException>(() => new PaymentInfo(cardNumber, past, cvv));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("12")]
    [InlineData("1234")]
    [InlineData("12a")]
    public void Constructor_Throws_WhenCVVInvalid(string badCvv)
    {
        var cardNumber = "1234567812345678";
        var expiration = DateTime.UtcNow.AddYears(1).ToString("MM/yy");
        Assert.Throws<ArgumentException>(() => new PaymentInfo(cardNumber, expiration, badCvv));
    }
}