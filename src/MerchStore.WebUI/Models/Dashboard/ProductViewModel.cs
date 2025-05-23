
using System.ComponentModel.DataAnnotations;

namespace MerchStore.WebUI.Models.Dashboard;

public class ProductViewModel
{
    public Guid Id { get; set; }
    
    [Required(ErrorMessage = "Product name is required")]
    [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
    public string Name { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Description is required")]
    [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
    public string Description { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, 10000, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }
    
    [Required(ErrorMessage = "Stock quantity is required")]
    [Range(0, 10000, ErrorMessage = "Stock must be between 0 and 10000")]
    public int Stock { get; set; }
    
    public string? ImageUrl { get; set; }
    
    public string Currency { get; set; } = "SEK";
}