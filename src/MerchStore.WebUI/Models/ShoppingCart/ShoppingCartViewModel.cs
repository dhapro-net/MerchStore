namespace MerchStore.WebUI.Models.ShoppingCart;

public class ShoppingCartViewModel
{
    public Guid CartId { get; set; }
    public List<ShoppingCartItemViewModel> Items { get; set; } = new List<ShoppingCartItemViewModel>();
    public decimal TotalPrice { get; set; }
    public int TotalItems { get; set; }
    public DateTime LastUpdated { get; set; }
    public ShippingDetailsViewModel Shipping { get; set; } = new ShippingDetailsViewModel();
    public PaymentDetailsViewModel Payment { get; set; } = new PaymentDetailsViewModel();
}
