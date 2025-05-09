namespace MerchStore.WebUI.Models.ShoppingCart;

public class ShoppingCartItemViewModel
{
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice => UnitPrice * Quantity;
    public string FormattedPrice => UnitPrice.ToString("C");
}