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