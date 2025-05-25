using MerchStore.Domain.Entities;

namespace MerchStore.Application.Services.Interfaces;


// Service interface for Review-related operations.
// Provides a simple abstraction over the repository layer.

public interface IReviewService
{
    
    Task<IEnumerable<Review>> GetReviewsByProductIdAsync(Guid productId);


    Task<double> GetAverageRatingForProductAsync(Guid productId);

    Task<int> GetReviewCountForProductAsync(Guid productId);

    Task AddReviewAsync(Review review);
}