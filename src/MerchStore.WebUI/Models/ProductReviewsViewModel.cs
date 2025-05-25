using MerchStore.Domain.Entities;

namespace MerchStore.WebUI.Models;

public class ProductReviewsViewModel
{
    public List<Product> Products { get; set; } = new List<Product>();
    public Dictionary<Guid, IEnumerable<Review>> ProductReviews { get; set; } = new Dictionary<Guid, IEnumerable<Review>>();
    public Dictionary<Guid, double> AverageRatings { get; set; } = new Dictionary<Guid, double>();
    public Dictionary<Guid, int> ReviewCounts { get; set; } = new Dictionary<Guid, int>();

    public Dictionary<Guid, string> ReviewSources { get; set; } = new();

}