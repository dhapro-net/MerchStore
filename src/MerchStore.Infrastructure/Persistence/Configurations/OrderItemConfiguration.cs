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
        builder.Property(oi => oi.Price).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(oi => oi.Quantity).IsRequired();

        // Foreign Key to Order
        builder.HasOne(oi => oi.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(oi => oi.OrderId);
    }
}