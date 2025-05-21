using MongoDB.Driver;
using Microsoft.Extensions.Logging;
using MerchStore.Domain.Entities;
using MerchStore.Domain.ValueObjects;

namespace MerchStore.Infrastructure.Persistence;

/// <summary>
/// Class for seeding the MongoDB database with initial data.
/// </summary>
public class MongoDbSeeder
{
    private readonly ILogger<MongoDbSeeder> _logger;
    private readonly IMongoCollection<Product> _products;

    public MongoDbSeeder(IMongoDatabase database, ILogger<MongoDbSeeder> logger)
    {
        _products = database.GetCollection<Product>("Products");
        _logger = logger;
    }

    public virtual async Task SeedAsync()
    {
        try
        {
            await SeedProductsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the MongoDB database.");
            throw;
        }
    }

    private async Task SeedProductsAsync()
    {
        var count = await _products.CountDocumentsAsync(FilterDefinition<Product>.Empty);
        if (count == 0)
        {
            _logger.LogInformation("Seeding products...");

            var products = new List<Product>
            {
                new Product(
                    Guid.NewGuid(),
                    "Canvas for decorating",
                    "A high-quality canvas for decorating your workspace.",
                    "Home Decoration",
                    "https://somethingpicture20250509.blob.core.windows.net/picture/canvas01.png",
                    Money.FromSEK(249.99m),
                    50),

                new Product(
                    Guid.NewGuid(),
                    "LittieST dragon Coaster",
                    "A coaster with a cute dragon design.",
                    "Home Decoration",
                    "https://somethingpicture20250509.blob.core.windows.net/picture/coaster.png",
                    Money.FromSEK(99.50m),
                    100),

                new Product(
                    Guid.NewGuid(),
                    "Hoodie",
                    "A comfortable hoodie with a cute design.",
                    "clothes",
                    "https://somethingpicture20250509.blob.core.windows.net/picture/hoodie.png",
                    Money.FromSEK(179.99m),
                    200),

                new Product(
                    Guid.NewGuid(),
                    "cute dragon sticker",
                    "A cute dragon sticker for your laptop or notebook.",
                    "stationery",
                    "https://somethingpicture20250509.blob.core.windows.net/picture/sticker.png",
                    Money.FromSEK(19.99m),
                    25)
            };

            await _products.InsertManyAsync(products);

            _logger.LogInformation("Product seeding completed successfully.");
        }
        else
        {
            _logger.LogInformation("MongoDB already contains products. Skipping product seed.");
        }
    }
}