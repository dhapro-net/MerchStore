public class CheckoutRequest
{
    public List<ShoppingCartItem> Items { get; set; }
    public ShippingInfo Shipping { get; set; }
    public PaymentInfo Payment { get; set; }
    public decimal TotalPrice { get; set; }
}

public class ShippingInfo
{
    public string FullName { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }
}

public class PaymentInfo
{
    public string CardNumber { get; set; }
    public string ExpirationDate { get; set; }
    public string CVV { get; set; }
}