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
                    name: "T-Shirt",
                    description: "A comfortable cotton t-shirt",
                    category: "Clothing",
                    imageUrl: "https://placehold.co/100x400.jpg",
                    price: new Money(19.99m, "SEK"),
                    stockQuantity: 100 // Provide stockQuantity here
                ),
                new Product(
                    name: "Mug",
                    description: "A ceramic mug for your coffee",
                    category: "Accessories",
                    imageUrl: "https://placehold.co/400x900.jpg",
                    price: new Money(9.99m, "SEK"),
                    stockQuantity: 50 // Provide stockQuantity here
                ),
                new Product(
                    name: "Notebook",
                    description: "A stylish notebook for your notes",
                    category: "Stationery",
                    imageUrl: "https://placehold.co/200x400.jpg",
                    price: new Money(14.99m, "SEK"),
                    stockQuantity: 75 // Provide stockQuantity here
                ),
                new Product(
                    name: "Backpack",
                    description: "A durable backpack for everyday use",
                    category: "Bags",
                    imageUrl: "https://placehold.co/400x600.jpg",
                    price: new Money(49.99m, "SEK"),
                    stockQuantity: 30 // Provide stockQuantity here
                ),
                new Product
                (
    name: "Hoodie",
    description: "A warm and cozy hoodie for chilly days",
    category: "Clothing",
    imageUrl: "https://placehold.co/432x400.jpg",
    price: new Money(29.99m, "SEK"),
    stockQuantity: 60
),
new Product(
    name: "Water Bottle",
    description: "A reusable stainless steel water bottle",
    category: "Accessories",
    imageUrl: "https://placehold.co/432x411.jpg",
    price: new Money(12.99m, "SEK"),
    stockQuantity: 120
),
new Product(
    name: "Desk Lamp",
    description: "An adjustable LED desk lamp with multiple brightness levels",
    category: "Electronics",
    imageUrl: "https://placehold.co/25x200.jpg",
    price: new Money(39.99m, "SEK"),
    stockQuantity: 40
),
new Product(
    name: "Gaming Mouse",
    description: "A high-precision gaming mouse with customizable buttons",
    category: "Electronics",
    imageUrl: "https://placehold.co/2000x400.jpg",
    price: new Money(24.99m, "SEK"),
    stockQuantity: 80
),
new Product(
    name: "Yoga Mat",
    description: "A non-slip yoga mat for your fitness routine",
    category: "Fitness",
    imageUrl: "https://placehold.co/400x400.jpg",
    price: new Money(19.99m, "SEK"),
    stockQuantity: 100
)
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