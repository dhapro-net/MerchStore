using MerchStore.Domain.Common;
using MerchStore.Domain.ValueObjects;
using System.IO;

namespace MerchStore.Domain.Entities;

public class Product : Entity<Guid>
{
    private static readonly string[] ValidImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;
    public Money Price { get; private set; } = Money.FromSEK(0);
    public int StockQuantity { get; private set; } = 0;
    public string ImageUrl { get; private set; } = string.Empty;
    public bool IsAvailable { get; private set; }
    public bool IsFeatured { get; private set; }

    private Product() { }

    public Product(Guid guid, string name, string description, string category, string imageUrl, Money price, int stockQuantity) : base(Guid.NewGuid())
    {
        SetName(name);
        SetDescription(description);
        SetCategory(category);
        SetImageUrl(imageUrl);
        SetPrice(price);
        SetStockQuantity(stockQuantity);
    }

    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be empty", nameof(name));

        if (name.Length > 100)
            throw new ArgumentException("Product name cannot exceed 100 characters", nameof(name));

        Name = name;
    }

    private void SetDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Product description cannot be empty", nameof(description));

        if (description.Length > 500)
            throw new ArgumentException("Product description cannot exceed 500 characters", nameof(description));

        Description = description;
    }

    private void SetCategory(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("Category cannot be empty", nameof(category));

        Category = category;
    }

    public void SetImageUrl(string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            throw new ArgumentException("Image URL cannot be empty", nameof(imageUrl));

        if (!Uri.TryCreate(imageUrl, UriKind.Absolute, out var uri) || (uri.Scheme != "http" && uri.Scheme != "https"))
            throw new ArgumentException("Image URL must be a valid HTTP or HTTPS URL", nameof(imageUrl));

        if (imageUrl.Length > 2000)
            throw new ArgumentException("Image URL exceeds maximum length of 2000 characters", nameof(imageUrl));

        string extension = Path.GetExtension(uri.AbsoluteUri).ToLowerInvariant();
        if (!ValidImageExtensions.Contains(extension))
            throw new ArgumentException("Image URL must point to a valid image file (jpg, jpeg, png, gif, webp)", nameof(imageUrl));

        ImageUrl = imageUrl;
    }

    private void SetPrice(Money price)
    {
        if (price is null)
            throw new ArgumentNullException(nameof(price));

        if (price.Amount <= 0)
            throw new ArgumentException("Price must be greater than zero", nameof(price));

        Price = price;
    }

    private void SetStockQuantity(int stockQuantity)
    {
        if (stockQuantity < 0)
            throw new ArgumentException("Stock quantity cannot be negative", nameof(stockQuantity));

        StockQuantity = stockQuantity;
        IsAvailable = stockQuantity > 0;
    }

    public void UpdateDetails(string name, string description, string imageUrl)
    {
        SetName(name);
        SetDescription(description);
        SetImageUrl(imageUrl);
    }

    public void UpdatePrice(Money newPrice)
    {
        SetPrice(newPrice);
    }

    public void UpdateStock(int quantity)
    {
        SetStockQuantity(quantity);
    }

    public bool DecrementStock(int quantity = 1)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        if (StockQuantity < quantity)
            return false;

        StockQuantity -= quantity;
        IsAvailable = StockQuantity > 0;
        return true;
    }

    public void IncrementStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        StockQuantity += quantity;
        IsAvailable = StockQuantity > 0;
    }
}