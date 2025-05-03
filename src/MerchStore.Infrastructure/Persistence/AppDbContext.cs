using Microsoft.EntityFrameworkCore;
using MerchStore.Domain.Entities;

namespace MerchStore.Infrastructure.Persistence;


public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }

    //Completed orders stored in database
    public DbSet<Order> Orders { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// This method is called when the model for a derived context is being created.
    /// It allows for configuration of entities, relationships, and other model-building activities.
    /// </summary>
    /// <param name="modelBuilder">Provides a simple API for configuring the model</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply entity configurations from the current assembly
        // This scans for all IEntityTypeConfiguration implementations and applies them
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}