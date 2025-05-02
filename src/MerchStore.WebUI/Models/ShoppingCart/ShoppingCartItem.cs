namespace MerchStore.WebUI.Models.ShoppingCart;

public class ShoppingCartItem
{
    public Guid ProductId { get; set; }
    public string Name { get; set; }
    public decimal PriceAmount { get; set; }
    public int Quantity { get; set; }
    public string? ImageUrl { get; set; }
}