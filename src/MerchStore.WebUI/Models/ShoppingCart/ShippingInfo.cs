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

