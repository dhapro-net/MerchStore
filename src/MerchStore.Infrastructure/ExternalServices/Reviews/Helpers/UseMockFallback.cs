using MerchStore.Application.DTOs;
using MerchStore.Domain.Entities;

namespace MerchStore.Infrastructure.ExternalServices.Reviews.Helpers;

public static class UseMockFallback
{
    public static ReviewResponseDto ToDto(Guid productId, IEnumerable<Review> reviews)
    {
        var reviewList = reviews.ToList();

        return new ReviewResponseDto
        {
            Reviews = reviewList.Select(r => new ReviewProductDto
            {
                Id = r.Id.ToString(),
                ProductId = r.ProductId.ToString(),
                CustomerName = r.CustomerName,
                Title = r.Title,
                Content = r.Content,
                Rating = r.Rating,
                CreatedAt = r.CreatedAt,
                Status = r.Status.ToString().ToLower()
            }).ToList(),

            Stats = new ReviewStatsDto
            {
                ProductId = productId.ToString(),
                AverageRating = reviewList.Any() ? reviewList.Average(r => r.Rating) : 0,
                ReviewCount = reviewList.Count
            },

            Source = "mock"
        };
    }
}
