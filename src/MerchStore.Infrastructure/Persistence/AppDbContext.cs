using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MerchStore.Domain.Entities;

namespace MerchStore.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<IdentityUser>
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

        modelBuilder.Entity<Order>(entity =>
        {
            entity.OwnsOne(o => o.PaymentInfo, paymentInfo =>
            {
                paymentInfo.Property(p => p.CardNumber).IsRequired();
                paymentInfo.Property(p => p.ExpirationDate).IsRequired();
                paymentInfo.Property(p => p.CVV).IsRequired();
            });
        });
        modelBuilder.Entity<OrderProduct>(entity =>
        {
            entity.HasKey(op => op.Id);

            entity.Property(op => op.ProductId).IsRequired();
            entity.Property(op => op.ProductName).IsRequired().HasMaxLength(255);
            entity.Property(op => op.Quantity).IsRequired();
            entity.Property(op => op.OrderId).IsRequired();

            entity.OwnsOne(op => op.UnitPrice, money =>
            {
                money.Property(m => m.Amount).HasColumnName("UnitPrice_Amount").IsRequired();
                money.Property(m => m.Currency).HasColumnName("UnitPrice_Currency").IsRequired();
            });
        });
    }
}