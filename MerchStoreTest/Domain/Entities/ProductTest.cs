using System;
using MerchStore.Domain.Entities;
using MerchStore.Domain.ValueObjects;
using Xunit;
namespace MerchStoreTest.Domain.Entities;

public class ProductTest
{
    private static Money ValidMoney() => new Money(100, "SEK");
    private static string ValidImageUrl() => "https://example.com/image.jpg";
    private static string ValidCategory() => "T-Shirts";
    private static string ValidDescription() => "A nice shirt";
    private static string ValidName() => "Cool Shirt";

    [Fact]
    public void Constructor_SetsProperties()
    {
        var id = Guid.NewGuid();
        var product = new Product(id, ValidName(), ValidDescription(), ValidCategory(), ValidImageUrl(), ValidMoney(), 10);

        Assert.Equal(id, product.Id);
        Assert.Equal(ValidName(), product.Name);
        Assert.Equal(ValidDescription(), product.Description);
        Assert.Equal(ValidCategory(), product.Category);
        Assert.Equal(ValidImageUrl(), product.ImageUrl);
        Assert.Equal(ValidMoney(), product.Price);
        Assert.Equal(10, product.StockQuantity);
        Assert.True(product.IsAvailable);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void SetName_Throws_WhenInvalid(string badName)
    {
        var id = Guid.NewGuid();
        Assert.Throws<ArgumentException>(() =>
            new Product(id, badName, ValidDescription(), ValidCategory(), ValidImageUrl(), ValidMoney(), 1));
    }

    [Fact]
    public void SetName_Throws_WhenTooLong()
    {
        var id = Guid.NewGuid();
        var longName = new string('a', 101);
        Assert.Throws<ArgumentException>(() =>
            new Product(id, longName, ValidDescription(), ValidCategory(), ValidImageUrl(), ValidMoney(), 1));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void SetDescription_Throws_WhenInvalid(string badDesc)
    {
        var id = Guid.NewGuid();
        Assert.Throws<ArgumentException>(() =>
            new Product(id, ValidName(), badDesc, ValidCategory(), ValidImageUrl(), ValidMoney(), 1));
    }

    [Fact]
    public void SetDescription_Throws_WhenTooLong()
    {
        var id = Guid.NewGuid();
        var longDesc = new string('a', 501);
        Assert.Throws<ArgumentException>(() =>
            new Product(id, ValidName(), longDesc, ValidCategory(), ValidImageUrl(), ValidMoney(), 1));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void SetCategory_Throws_WhenInvalid(string badCat)
    {
        var id = Guid.NewGuid();
        Assert.Throws<ArgumentException>(() =>
            new Product(id, ValidName(), ValidDescription(), badCat, ValidImageUrl(), ValidMoney(), 1));
    }

    [Theory]
    [InlineData("not-a-url")]
    [InlineData("ftp://example.com/image.jpg")]
    [InlineData("https://example.com/image.txt")]
    public void SetImageUrl_Throws_WhenInvalid(string badUrl)
    {
        var id = Guid.NewGuid();
        Assert.Throws<ArgumentException>(() =>
            new Product(id, ValidName(), ValidDescription(), ValidCategory(), badUrl, ValidMoney(), 1));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void SetImageUrl_Throws_WhenEmpty(string badUrl)
    {
        var id = Guid.NewGuid();
        Assert.Throws<ArgumentException>(() =>
            new Product(id, ValidName(), ValidDescription(), ValidCategory(), badUrl, ValidMoney(), 1));
    }

    [Fact]
    public void SetImageUrl_Throws_WhenTooLong()
    {
        var id = Guid.NewGuid();
        var longUrl = "https://example.com/" + new string('a', 2000) + ".jpg";
        Assert.Throws<ArgumentException>(() =>
            new Product(id, ValidName(), ValidDescription(), ValidCategory(), longUrl, ValidMoney(), 1));
    }

    [Fact]
    public void SetPrice_Throws_WhenNull()
    {
        var id = Guid.NewGuid();
        Assert.Throws<ArgumentNullException>(() =>
            new Product(id, ValidName(), ValidDescription(), ValidCategory(), ValidImageUrl(), null, 1));
    }

    [Fact]
    public void SetPrice_Throws_WhenZeroOrNegative()
    {
        var id = Guid.NewGuid();
        Assert.Throws<ArgumentException>(() =>
            new Product(id, ValidName(), ValidDescription(), ValidCategory(), ValidImageUrl(), new Money(0, "SEK"), 1));
        Assert.Throws<ArgumentException>(() =>
            new Product(id, ValidName(), ValidDescription(), ValidCategory(), ValidImageUrl(), new Money(-5, "SEK"), 1));
    }

    [Fact]
    public void SetStockQuantity_Throws_WhenNegative()
    {
        var id = Guid.NewGuid();
        Assert.Throws<ArgumentException>(() =>
            new Product(id, ValidName(), ValidDescription(), ValidCategory(), ValidImageUrl(), ValidMoney(), -1));
    }

    [Fact]
    public void UpdateDetails_UpdatesNameDescriptionAndImageUrl()
    {
        var product = new Product(Guid.NewGuid(), ValidName(), ValidDescription(), ValidCategory(), ValidImageUrl(), ValidMoney(), 1);
        var newName = "New Name";
        var newDesc = "New Description";
        var newUrl = "https://example.com/new.jpg";

        product.UpdateDetails(newName, newDesc, newUrl);

        Assert.Equal(newName, product.Name);
        Assert.Equal(newDesc, product.Description);
        Assert.Equal(newUrl, product.ImageUrl);
    }

    [Fact]
    public void UpdatePrice_UpdatesPrice()
    {
        var product = new Product(Guid.NewGuid(), ValidName(), ValidDescription(), ValidCategory(), ValidImageUrl(), ValidMoney(), 1);
        var newPrice = new Money(200, "SEK");

        product.UpdatePrice(newPrice);

        Assert.Equal(newPrice, product.Price);
    }

    [Fact]
    public void UpdateStock_UpdatesStockQuantityAndAvailability()
    {
        var product = new Product(Guid.NewGuid(), ValidName(), ValidDescription(), ValidCategory(), ValidImageUrl(), ValidMoney(), 1);

        product.UpdateStock(0);
        Assert.Equal(0, product.StockQuantity);
        Assert.False(product.IsAvailable);

        product.UpdateStock(5);
        Assert.Equal(5, product.StockQuantity);
        Assert.True(product.IsAvailable);
    }

    [Fact]
    public void DecrementStock_DecreasesStockAndReturnsTrue_WhenEnoughStock()
    {
        var product = new Product(Guid.NewGuid(), ValidName(), ValidDescription(), ValidCategory(), ValidImageUrl(), ValidMoney(), 5);

        var result = product.DecrementStock(3);

        Assert.True(result);
        Assert.Equal(2, product.StockQuantity);
        Assert.True(product.IsAvailable);
    }

    [Fact]
    public void DecrementStock_ReturnsFalse_WhenNotEnoughStock()
    {
        var product = new Product(Guid.NewGuid(), ValidName(), ValidDescription(), ValidCategory(), ValidImageUrl(), ValidMoney(), 2);

        var result = product.DecrementStock(3);

        Assert.False(result);
        Assert.Equal(2, product.StockQuantity);
        Assert.True(product.IsAvailable);
    }

    [Fact]
    public void DecrementStock_SetsIsAvailableFalse_WhenStockBecomesZero()
    {
        var product = new Product(Guid.NewGuid(), ValidName(), ValidDescription(), ValidCategory(), ValidImageUrl(), ValidMoney(), 1);

        var result = product.DecrementStock(1);

        Assert.True(result);
        Assert.Equal(0, product.StockQuantity);
        Assert.False(product.IsAvailable);
    }

    [Fact]
    public void DecrementStock_Throws_WhenQuantityIsNotPositive()
    {
        var product = new Product(Guid.NewGuid(), ValidName(), ValidDescription(), ValidCategory(), ValidImageUrl(), ValidMoney(), 5);

        Assert.Throws<ArgumentException>(() => product.DecrementStock(0));
        Assert.Throws<ArgumentException>(() => product.DecrementStock(-1));
    }

    [Fact]
    public void IncrementStock_IncreasesStockAndSetsIsAvailable()
    {
        var product = new Product(Guid.NewGuid(), ValidName(), ValidDescription(), ValidCategory(), ValidImageUrl(), ValidMoney(), 0);

        product.IncrementStock(3);

        Assert.Equal(3, product.StockQuantity);
        Assert.True(product.IsAvailable);
    }

    [Fact]
    public void IncrementStock_Throws_WhenQuantityIsNotPositive()
    {
        var product = new Product(Guid.NewGuid(), ValidName(), ValidDescription(), ValidCategory(), ValidImageUrl(), ValidMoney(), 1);

        Assert.Throws<ArgumentException>(() => product.IncrementStock(0));
        Assert.Throws<ArgumentException>(() => product.IncrementStock(-5));
    }
}