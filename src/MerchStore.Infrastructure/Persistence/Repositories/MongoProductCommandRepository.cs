using MongoDB.Driver;
using MerchStore.Domain.Entities;
using MerchStore.Domain.Interfaces;

namespace MerchStore.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository for managing Product entities commands using MongoDB.
/// </summary>
public class MongoProductCommandRepository : IProductCommandRepository, ICommandRepository<Product, Guid>
{
    private readonly IMongoCollection<Product> _products;
    private readonly IMongoCollection<Order> _orders;

    public MongoProductCommandRepository(IMongoDatabase database)
    {
        _products = database.GetCollection<Product>("Products");
        _orders = database.GetCollection<Order>("Orders");
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

     public async Task AddAsync(Product entity)
    {
        await _products.InsertOneAsync(entity);
    }

    public async Task UpdateAsync(Product entity)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.Id, entity.Id);
        await _products.ReplaceOneAsync(filter, entity);
    }

    public async Task RemoveAsync(Product entity)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.Id, entity.Id);
        await _products.DeleteOneAsync(filter);
    }
}