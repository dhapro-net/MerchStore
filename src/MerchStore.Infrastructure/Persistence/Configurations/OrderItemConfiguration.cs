using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MerchStore.Domain.Entities;

namespace MerchStore.Infrastructure.Persistence.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        // Primary Key
        builder.HasKey(oi => oi.Id);

        // Product Information
        builder.Property(oi => oi.ProductId).IsRequired();
        builder.Property(oi => oi.ProductName).IsRequired().HasMaxLength(100);
        // Configure the Money value object for Price
        builder.OwnsOne(oi => oi.Price, price =>
        {
            price.Property(p => p.Amount).HasColumnType("decimal(18,2)").IsRequired();
            price.Property(p => p.Currency).HasMaxLength(3).IsRequired();
        });

        // Foreign Key to Order
        builder.HasOne(oi => oi.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(oi => oi.OrderId);
    }
}