using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MerchStore.Domain.Entities;

namespace MerchStore.Infrastructure.Persistence.Configurations;

public class OrderProductConfiguration : IEntityTypeConfiguration<OrderProduct>
{
    public void Configure(EntityTypeBuilder<OrderProduct> builder)
    {
        builder.ToTable("OrderProducts");

        // Primary Key
        builder.HasKey(oi => oi.Id);

        // Product Information
        builder.Property(oi => oi.ProductId).IsRequired();
        builder.Property(oi => oi.ProductName).IsRequired().HasMaxLength(100);
        // Configure the Money value object for Price
        builder.OwnsOne(oi => oi.UnitPrice, price =>
        {
            price.Property(p => p.Amount).HasColumnType("decimal(18,2)").IsRequired();
            price.Property(p => p.Currency).HasMaxLength(3).IsRequired();
        });

        // Foreign Key to Order
        builder.HasOne(oi => oi.Order)
            .WithMany(o => o.Products)
            .HasForeignKey(oi => oi.OrderId);
    }
}