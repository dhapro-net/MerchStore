using MongoDB.Driver;
using MerchStore.Domain.Entities;
using MerchStore.Domain.Interfaces;

namespace MerchStore.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository for querying Product entities using MongoDB.
/// </summary>
public class MongoProductQueryRepository : IProductQueryRepository
{
    private readonly IMongoCollection<Product> _products;

    public MongoProductQueryRepository(IMongoDatabase database)
    {
        _products = database.GetCollection<Product>("Products");
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync(CancellationToken cancellationToken)
    {
        var result = await _products.Find(_ => true).ToListAsync(cancellationToken);
        return result;
    }

    public async Task<Product?> GetProductByIdAsync(Guid productId, CancellationToken cancellationToken)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.Id, productId);
        return await _products.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetFeaturedProductsAsync()
    {
        var filter = Builders<Product>.Filter.Eq(p => p.IsFeatured, true);
        return await _products.Find(filter).ToListAsync();
    }

    public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await _products.Find(_ => true).ToListAsync();

        var filter = Builders<Product>.Filter.Or(
            Builders<Product>.Filter.Regex(p => p.Name, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
            Builders<Product>.Filter.Regex(p => p.Description, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"))
        );
        return await _products.Find(filter).ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.Category, category);
        return await _products.Find(filter).ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsInPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        var filter = Builders<Product>.Filter.And(
            Builders<Product>.Filter.Gte("Price.Amount", minPrice),
            Builders<Product>.Filter.Lte("Price.Amount", maxPrice)
        );
        return await _products.Find(filter).ToListAsync();
    }
}