using MerchStore.Domain.ValueObjects;

public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Money Price { get; set; }
    public Uri ImageUrl { get; set; }
    public int StockQuantity { get; set; }

    public bool InStock => StockQuantity > 0; 
}