namespace MerchStore.WebUI.Models.ShoppingCart;

public class ShoppingCartViewModel
{
    public List<ShoppingCartItemViewModel> Items { get; set; }
    public decimal TotalPrice { get; set; }
    public ShippingInfo Shipping { get; set; } = new ShippingInfo();
    public PaymentInfo Payment { get; set; } = new PaymentInfo();
}