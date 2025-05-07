using System.ComponentModel.DataAnnotations;

namespace MerchStore.WebUI.Models.ShoppingCart
{
    public class PaymentDetailsViewModel
    {
        [Required(ErrorMessage = "Card number is required.")]
        [CreditCard(ErrorMessage = "Invalid card number.")]
        public string CardNumber { get; set; }

        [Required(ErrorMessage = "Expiration date is required.")]
        [RegularExpression(@"^(0[1-9]|1[0-2])\/?([0-9]{2})$", ErrorMessage = "Invalid expiration date. Use MM/YY format.")]
        public string ExpirationDate { get; set; }

        [Required(ErrorMessage = "CVV is required.")]
        [RegularExpression(@"^\d{3}$", ErrorMessage = "Invalid CVV. Must be 3 digits.")]
        public string? CVV { get; set; }
    }
}