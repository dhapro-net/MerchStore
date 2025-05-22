namespace MerchStore.WebUI.Models.ShoppingCart;

public class ShoppingCartProductViewModel
{
    public required string ProductId { get; set; }
    public required string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice => UnitPrice * Quantity;
    public string FormattedPrice => UnitPrice.ToString("C");
}