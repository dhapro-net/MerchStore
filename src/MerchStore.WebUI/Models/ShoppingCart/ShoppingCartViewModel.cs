namespace MerchStore.WebUI.Models.ShoppingCart;

public class ShoppingCartViewModel
{
    public List<ShoppingCartItemViewModel> Items { get; set; }
    public decimal TotalPrice { get; set; }
    public ShippingDetailsViewModel Shipping { get; set; } = new ShippingDetailsViewModel();
    public PaymentDetailsViewModel Payment { get; set; } = new PaymentDetailsViewModel();
}