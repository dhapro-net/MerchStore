namespace MerchStore.Infrastructure.ExternalServices.Reviews.Models;

public class ReviewContentDto
{
    public string? ReviewId { get; set; }
    public string? ProductId { get; set; }
    public string? ReviewerName { get; set; }
    public string? ReviewTitle { get; set; }
    public string? ReviewContent { get; set; }
    public int Rating { get; set; }
    public DateTime CreationDate { get; set; }
}

