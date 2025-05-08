public class CartDto
{
    public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
}

public class CartItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string ImageUrl { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}