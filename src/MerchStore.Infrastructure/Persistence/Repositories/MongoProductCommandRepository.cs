using MongoDB.Driver;
using MerchStore.Domain.Entities;
using MerchStore.Domain.Interfaces;

namespace MerchStore.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository for managing Product entities commands using MongoDB.
/// </summary>
public class MongoProductCommandRepository : IProductCommandRepository
{
    private readonly IMongoCollection<Product> _products;

    public MongoProductCommandRepository(IMongoDatabase database)
    {
        _products = database.GetCollection<Product>("Products");
    }

    public async Task<bool> IsInStockAsync(Guid productId, int quantity)
    {
        var filter = Builders<Product>.Filter.And(
            Builders<Product>.Filter.Eq(p => p.Id, productId),
            Builders<Product>.Filter.Gte(p => p.StockQuantity, quantity)
        );
        var product = await _products.Find(filter).FirstOrDefaultAsync();
        return product != null;
    }

    public async Task<bool> UpdateStockAsync(Guid productId, int quantity, CancellationToken cancellationToken)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.Id, productId);
        var update = Builders<Product>.Update.Set(p => p.StockQuantity, quantity);

        var result = await _products.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);

        return result.ModifiedCount > 0;
    }
}