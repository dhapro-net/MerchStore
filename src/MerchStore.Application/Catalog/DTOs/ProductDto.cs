using MerchStore.Domain.ValueObjects;

public class ProductDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required Money Price { get; set; }
    public required Uri ImageUrl { get; set; }
    public int StockQuantity { get; set; }

    public bool InStock => StockQuantity > 0; 
}