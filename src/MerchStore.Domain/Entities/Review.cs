using MerchStore.Domain.Common;
using MerchStore.Domain.Enums;

namespace MerchStore.Domain.Entities;

public class Review : Entity<Guid>
{
    // Properties with private setters for encapsulation
    public Guid ProductId { get; private set; }
    public string CustomerName { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
    public int Rating { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public ReviewStatus Status { get; private set; }

    // Private parameterless constructor for EF Core
    private Review() { }

    // Public constructor with required parameters
    public Review(
        Guid id,
        Guid productId,
        string customerName,
        string title,
        string content,
        int rating,
        DateTime createdAt,
        ReviewStatus status) : base(id)
    {
        // Validate parameters
        if (productId == Guid.Empty)
            throw new ArgumentException("Product ID cannot be empty", nameof(productId));

        if (string.IsNullOrWhiteSpace(customerName))
            throw new ArgumentException("Customer name cannot be empty", nameof(customerName));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Review title cannot be empty", nameof(title));

        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Review content cannot be empty", nameof(content));

        if (rating < 1 || rating > 5)
            throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between 1 and 5");

        // Set properties
        ProductId = productId;
        CustomerName = customerName;
        Title = title;
        Content = content;
        Rating = rating;
        CreatedAt = createdAt;
        Status = status;
    }
}