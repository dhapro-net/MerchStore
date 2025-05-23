namespace MerchStore.Infrastructure.ExternalServices.Reviews.Models;

public class ProductReviewsDto
{
    public string? ProductId { get; set; }
    public string? ProductName { get; set; }
    public List<ReviewContentDto>? Reviews { get; set; }
}
