
using MerchStore.Domain.Entities;
using MerchStore.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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

        // Total Price with Value Converter
        var moneyConverter = new ValueConverter<Money, decimal>(
            v => v.Amount, // Convert Money to decimal for the database
            v => new Money(v, "SEK") // Convert decimal back to Money for the application
        );

        // Total Price
        builder.Property(o => o.TotalPrice)
            .IsRequired()
            .HasColumnType("decimal(18,2)")
            .HasConversion(moneyConverter);

        // Configure the relationship with OrderItem
        builder.HasMany(o => o.Items)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure the owned PaymentInfo value object
        builder.OwnsOne(o => o.PaymentInfo, paymentInfo =>
        {
            paymentInfo.Property(p => p.CardNumber).HasMaxLength(16).IsRequired();
            paymentInfo.Property(p => p.ExpirationDate).HasMaxLength(5).IsRequired();
            paymentInfo.Property(p => p.CVV).HasMaxLength(3).IsRequired();
        });
    }
}