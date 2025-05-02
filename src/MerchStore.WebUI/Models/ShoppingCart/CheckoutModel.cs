public class CheckoutRequest
{
    public List<ShoppingCartItem> Items { get; set; }
    public ShippingInfo Shipping { get; set; }
    public PaymentInfo Payment { get; set; }
    public decimal TotalPrice { get; set; }
}