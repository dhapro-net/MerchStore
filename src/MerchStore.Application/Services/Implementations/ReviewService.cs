using MerchStore.Application.Services.Interfaces;
using MerchStore.Domain.Entities;
using MerchStore.Domain.Interfaces;

namespace MerchStore.Application.Services.Implementations;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepository;

    public ReviewService(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<IEnumerable<Review>> GetReviewsByProductIdAsync(Guid productId)
    {
        var (reviews, _) = await _reviewRepository.GetProductReviewsAsync(productId);
        return reviews;
    }

    public async Task<double> GetAverageRatingForProductAsync(Guid productId)
    {
        var (_, stats) = await _reviewRepository.GetProductReviewsAsync(productId);
        return stats.AverageRating;
    }

    public async Task<int> GetReviewCountForProductAsync(Guid productId)
    {
        var (_, stats) = await _reviewRepository.GetProductReviewsAsync(productId);
        return stats.ReviewCount;
    }

    public Task AddReviewAsync(Review review)
    {
        // Since the IReviewRepository doesn't have an AddAsync method,
        // we need to figure out how to handle this case
        throw new NotImplementedException("Adding reviews is not supported by the current repository interface.");
    }
}