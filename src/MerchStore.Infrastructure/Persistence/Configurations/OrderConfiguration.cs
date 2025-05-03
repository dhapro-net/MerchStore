using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MerchStore.Domain.Entities;

namespace MerchStore.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        // Primary Key
        builder.HasKey(o => o.Id);

        // Customer Information
        builder.Property(o => o.CustomerName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(o => o.Address)
            .IsRequired()
            .HasMaxLength(200);

        // Total Price
        builder.Property(o => o.TotalPrice)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        // Configure the relationship with OrderItem
        builder.HasMany(o => o.Items)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure the owned PaymentInfo value object
        builder.OwnsOne(o => o.Payment, payment =>
        {
            payment.Property(p => p.CardNumber).HasMaxLength(16).IsRequired();
            payment.Property(p => p.ExpirationDate).HasMaxLength(5).IsRequired();
            payment.Property(p => p.CVV).HasMaxLength(3).IsRequired();
        });
    }
}