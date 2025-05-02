public class ShippingInfo
{
    [Required]
    public string FullName { get; set; }

    [Required]
    public string Address { get; set; }

    [Required]
    public string City { get; set; }

    [Required]
    [RegularExpression(@"^\d{5}(-\d{4})?$", ErrorMessage = "Invalid postal code.")]
    public string PostalCode { get; set; }

    [Required]
    public string Country { get; set; }
}

public class PaymentInfo
{
    [Required]
    [CreditCard]
    public string CardNumber { get; set; }

    [Required]
    [RegularExpression(@"^(0[1-9]|1[0-2])\/?([0-9]{2})$", ErrorMessage = "Invalid expiration date.")]
    public string ExpirationDate { get; set; }

    [Required]
    [RegularExpression(@"^\d{3,4}$", ErrorMessage = "Invalid CVV.")]
    public string CVV { get; set; }
}