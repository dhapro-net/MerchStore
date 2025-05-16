namespace MerchStore.WebUI.Models.ShoppingCart;

public class ShoppingCartViewModel
{
    public Guid CartId { get; set; }
    public List<ShoppingCartProductViewModel> Products { get; set; } = new List<ShoppingCartProductViewModel>();
    public decimal TotalPrice { get; set; }
    public int TotalProducts { get; set; }
    public DateTime LastUpdated { get; set; }
    public ShippingDetailsViewModel Shipping { get; set; } = new ShippingDetailsViewModel();
    public PaymentDetailsViewModel Payment { get; set; } = new PaymentDetailsViewModel
    {
        CardNumber = string.Empty,
        ExpirationDate = string.Empty
    };
}
