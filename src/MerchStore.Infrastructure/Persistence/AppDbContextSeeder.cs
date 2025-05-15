using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MerchStore.Domain.Entities;
using MerchStore.Domain.ValueObjects;

namespace MerchStore.Infrastructure.Persistence;

/// <summary>
/// Class for seeding the database with initial data.
/// This is useful for development, testing, and demos.
/// </summary>
public class AppDbContextSeeder
{
    private readonly ILogger<AppDbContextSeeder> _logger;
    private readonly AppDbContext _context;

    /// <summary>
    /// Constructor that accepts the context and a logger
    /// </summary>
    /// <param name="context">The database context to seed</param>
    /// <param name="logger">The logger for logging seed operations</param>
    public AppDbContextSeeder(AppDbContext context, ILogger<AppDbContextSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Seeds the database with initial data
    /// </summary>
    public virtual async Task SeedAsync()
    {
        try
        {
            // Ensure the database is created (only needed for in-memory database)
            // For SQL Server, you would use migrations instead
            await _context.Database.EnsureCreatedAsync();

            // Seed products if none exist
            await SeedProductsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    /// <summary>
    /// Seeds the database with sample products
    /// </summary>
    private async Task SeedProductsAsync()
    {
        // Check if we already have products (to avoid duplicate seeding)
        if (!await _context.Products.AnyAsync())
        {
            _logger.LogInformation("Seeding products...");

// Add sample products
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
        "Littier dragon Coaster",
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

            await _context.Products.AddRangeAsync(products);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Product seeding completed successfully.");
        }
        else
        {
            _logger.LogInformation("Database already contains products. Skipping product seed.");
        }
    }


}