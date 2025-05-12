using MerchStore.Domain.Entities;

namespace MerchStore.WebUI.Models;

public class ProductReviewViewModel
{
    public Product Product { get; set; } = null!;
    public List<Review> Reviews { get; set; } = new List<Review>();
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
}