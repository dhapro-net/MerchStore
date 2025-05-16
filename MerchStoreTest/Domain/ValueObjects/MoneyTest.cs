using System;
using MerchStore.Domain.ValueObjects;
using Xunit;
namespace MerchStoreTest.Domain.ValueObjects;
public class MoneyTest
{
    [Fact]
    public void Constructor_SetsProperties_AndUppercasesCurrency()
    {
        var money = new Money(123.45m, "sek");
        Assert.Equal(123.45m, money.Amount);
        Assert.Equal("SEK", money.Currency);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Constructor_Throws_WhenAmountNegative(decimal badAmount)
    {
        Assert.Throws<ArgumentException>(() => new Money(badAmount, "SEK"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_Throws_WhenCurrencyNullOrEmpty(string badCurrency)
    {
        Assert.Throws<ArgumentException>(() => new Money(10, badCurrency));
    }

    [Theory]
    [InlineData("SE")]
    [InlineData("SEKK")]
    [InlineData("US")]
    [InlineData("EURO")]
    public void Constructor_Throws_WhenCurrencyNotThreeLetters(string badCurrency)
    {
        Assert.Throws<ArgumentException>(() => new Money(10, badCurrency));
    }

    [Fact]
    public void FromSEK_ReturnsMoneyWithSEK()
    {
        var money = Money.FromSEK(50);
        Assert.Equal(50, money.Amount);
        Assert.Equal("SEK", money.Currency);
    }

    [Fact]
    public void Addition_Works_WhenSameCurrency()
    {
        var m1 = new Money(10, "SEK");
        var m2 = new Money(15, "SEK");
        var sum = m1 + m2;
        Assert.Equal(25, sum.Amount);
        Assert.Equal("SEK", sum.Currency);
    }

    [Fact]
    public void Addition_Throws_WhenDifferentCurrency()
    {
        var m1 = new Money(10, "SEK");
        var m2 = new Money(15, "USD");
        Assert.Throws<InvalidOperationException>(() => { var _ = m1 + m2; });
    }

    [Fact]
    public void Multiply_ByInt_Works()
    {
        var m = new Money(10, "SEK");
        var result = m * 3;
        Assert.Equal(30, result.Amount);
        Assert.Equal("SEK", result.Currency);
    }

    [Fact]
    public void Multiply_ByDecimal_Works()
    {
        var m = new Money(10, "SEK");
        var result = m * 2.5m;
        Assert.Equal(25, result.Amount);
        Assert.Equal("SEK", result.Currency);
    }

    [Fact]
    public void Multiply_ByNegativeDecimal_Throws()
    {
        var m = new Money(10, "SEK");
        Assert.Throws<ArgumentException>(() => { var _ = m * -1m; });
    }

    [Fact]
    public void Multiply_IntLeft_Works()
    {
        var m = new Money(10, "SEK");
        var result = 4 * m;
        Assert.Equal(40, result.Amount);
        Assert.Equal("SEK", result.Currency);
    }

    [Fact]
    public void Multiply_DecimalLeft_Works()
    {
        var m = new Money(10, "SEK");
        var result = 1.5m * m;
        Assert.Equal(15, result.Amount);
        Assert.Equal("SEK", result.Currency);
    }

    [Fact]
    public void ToString_FormatsCorrectly()
    {
        var m = new Money(123.456m, "SEK");
        Assert.Equal("123.46 SEK", m.ToString());
    }
}