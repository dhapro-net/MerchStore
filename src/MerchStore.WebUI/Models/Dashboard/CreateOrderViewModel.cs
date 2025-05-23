using System.ComponentModel.DataAnnotations;

namespace MerchStore.WebUI.Models.Dashboard;


public class CreateOrderViewModel
    {
        [Required]
        [Display(Name = "Customer Name")]
        public required string CustomerName { get; set; }
        
        [Required]
        [EmailAddress]
        [Display(Name = "Customer Email")]
        public required string CustomerEmail { get; set; }
        
        [Required]
        [Display(Name = "Street Address")]
        public required string StreetAddress { get; set; }
        
        [Required]
        public required string City { get; set; }
        
        [Required]
        [Display(Name = "Zip Code")]
        public required string ZipCode { get; set; }
        
        [Required]
        public required string Country { get; set; }
    }