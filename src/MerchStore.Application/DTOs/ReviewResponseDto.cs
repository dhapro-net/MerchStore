


namespace MerchStore.Application.DTOs;

public class ReviewResponseDto
{
    public List<ReviewProductDto>? Reviews { get; set; }
    public ReviewStatsDto? Stats { get; set; }

    public string Source { get; set; } = "real";

}