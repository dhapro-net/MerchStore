
namespace MerchStore.Application.DTOs;

public class ReviewStatsDto
{
    public string? ProductId { get; set; }
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }

    public string Source { get; set; } = "real";

}