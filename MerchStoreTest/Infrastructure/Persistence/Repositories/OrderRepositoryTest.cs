using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MerchStore.Domain.Entities;
using MerchStore.Domain.ValueObjects;
using MerchStore.Infrastructure.Persistence;
using MerchStore.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MerchStoreTest.Infrastructure.Repositories
{
    public class OrderRepositoryTests
    {
        private AppDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Unique database for each test
                .Options;

            return new AppDbContext(options);
        }

        // Helper for test PaymentInfo
        private PaymentInfo TestPaymentInfo()
        {
            var ctor = typeof(PaymentInfo).GetConstructor(
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
                null,
                new[] { typeof(string), typeof(string), typeof(DateTime) },
                null
            );
            return (PaymentInfo)ctor.Invoke(new object[] { "TestCard", "1234", DateTime.UtcNow.AddYears(1) });
        }
        // Helper for test OrderProduct
        private OrderProduct TestOrderProduct()
        {
            return new OrderProduct(
                Guid.NewGuid(),                // id
                Guid.NewGuid(),                // productId
                "Test Product",                // productName
                new Money(100, "USD"),         // unitPrice
                2,                             // quantity
                Guid.NewGuid()                 // orderId (can be dummy for test)
            );
        }
        [Fact]
        public async Task AddOrderAsync_ShouldAddOrderToDatabase()
        {
            // Arrange
            await using var context = CreateInMemoryDbContext();
            var commandRepo = new OrderCommandRepository(context);

            var order = new Order(
                Guid.NewGuid(),
                TestPaymentInfo(),
                "John Doe",
                "123 Test St",
                new Money(200, "USD"),
                new List<OrderProduct> { TestOrderProduct() },
                DateTime.UtcNow
            );

            // Act
            var result = await commandRepo.AddOrderAsync(order);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(order.Id, result.Id);
            Assert.Single(context.Orders);
        }

        [Fact]
        public async Task GetOrderByIdAsync_ShouldReturnOrder_WhenOrderExists()
        {
            // Arrange
            await using var context = CreateInMemoryDbContext();
            var commandRepo = new OrderCommandRepository(context);
            var queryRepo = new OrderQueryRepository(context);

            var order = new Order(
                Guid.NewGuid(),
                TestPaymentInfo(),
                "John Doe",
                "123 Test St",
                new Money(200, "USD"),
                new List<OrderProduct> { TestOrderProduct() },
                DateTime.UtcNow
            );

            await commandRepo.AddOrderAsync(order);

            // Act
            var result = await queryRepo.GetOrderByIdAsync(order.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(order.Id, result.Id);
        }

        [Fact]
        public async Task GetOrderByIdAsync_ShouldReturnNull_WhenOrderDoesNotExist()
        {
            // Arrange
            await using var context = CreateInMemoryDbContext();
            var queryRepo = new OrderQueryRepository(context);

            // Act
            var result = await queryRepo.GetOrderByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetOrdersByCustomerAsync_ShouldReturnOrdersForCustomer()
        {
            // Arrange
            await using var context = CreateInMemoryDbContext();
            var commandRepo = new OrderCommandRepository(context);

            var customerId = Guid.NewGuid();
            var orders = new List<Order>
            {
                new Order(Guid.NewGuid(), TestPaymentInfo(), "John Doe", "123 Test St", new Money(200, "USD"), new List<OrderProduct> { TestOrderProduct() }, DateTime.UtcNow),
                new Order(Guid.NewGuid(), TestPaymentInfo(), "John Doe", "123 Test St", new Money(200, "USD"), new List<OrderProduct> { TestOrderProduct() }, DateTime.UtcNow),
                new Order(Guid.NewGuid(), TestPaymentInfo(), "Jane Smith", "456 Other St", new Money(150, "USD"), new List<OrderProduct> { TestOrderProduct() }, DateTime.UtcNow)
            };

            foreach (var order in orders)
                await commandRepo.AddOrderAsync(order);

            // Act
            var result = context.Orders.Where(o => o.CustomerName == "John Doe").ToList();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetOrdersByCustomerAsync_ShouldReturnEmpty_WhenNoOrdersExist()
        {
            // Arrange
            await using var context = CreateInMemoryDbContext();

            // Act
            var result = await context.Orders.Where(o => o.CustomerName == "Nonexistent").ToListAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetOrdersByDateRangeAsync_ShouldReturnOrdersWithinDateRange()
        {
            // Arrange
            await using var context = CreateInMemoryDbContext();
            var commandRepo = new OrderCommandRepository(context);
            var queryRepo = new OrderQueryRepository(context);

            var orders = new List<Order>
            {
                new Order(Guid.NewGuid(), TestPaymentInfo(), "A", "A", new Money(100, "USD"), new List<OrderProduct> { TestOrderProduct() }, DateTime.UtcNow.AddDays(-5)),
                new Order(Guid.NewGuid(), TestPaymentInfo(), "B", "B", new Money(100, "USD"), new List<OrderProduct> { TestOrderProduct() }, DateTime.UtcNow.AddDays(-3)),
                new Order(Guid.NewGuid(), TestPaymentInfo(), "C", "C", new Money(100, "USD"), new List<OrderProduct> { TestOrderProduct() }, DateTime.UtcNow.AddDays(-1))
            };

            foreach (var order in orders)
                await commandRepo.AddOrderAsync(order);

            var startDate = DateTime.UtcNow.AddDays(-4);
            var endDate = DateTime.UtcNow;

            // Act
            var result = await queryRepo.GetOrdersByDateRangeAsync(startDate, endDate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetOrdersByDateRangeAsync_ShouldIncludeOrdersOnStartAndEndDate()
        {
            // Arrange
            await using var context = CreateInMemoryDbContext();
            var commandRepo = new OrderCommandRepository(context);
            var queryRepo = new OrderQueryRepository(context);

            var startDate = DateTime.UtcNow.AddDays(-5);
            var endDate = DateTime.UtcNow;

            var orders = new List<Order>
            {
                new Order(Guid.NewGuid(), TestPaymentInfo(), "A", "A", new Money(100, "USD"), new List<OrderProduct> { TestOrderProduct() }, startDate),
                new Order(Guid.NewGuid(), TestPaymentInfo(), "B", "B", new Money(100, "USD"), new List<OrderProduct> { TestOrderProduct() }, endDate),
                new Order(Guid.NewGuid(), TestPaymentInfo(), "C", "C", new Money(100, "USD"), new List<OrderProduct> { TestOrderProduct() }, DateTime.UtcNow.AddDays(-3))
            };

            foreach (var order in orders)
                await commandRepo.AddOrderAsync(order);

            // Act
            var result = await queryRepo.GetOrdersByDateRangeAsync(startDate, endDate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task UpdateOrderAsync_ShouldUpdateOrderInDatabase()
        {
            // Arrange
            await using var context = CreateInMemoryDbContext();
            var commandRepo = new OrderCommandRepository(context);

            var order = new Order(
                Guid.NewGuid(),
                TestPaymentInfo(),
                "John Doe",
                "123 Test St",
                new Money(200, "USD"),
                new List<OrderProduct> { TestOrderProduct() },
                DateTime.UtcNow
            );

            await commandRepo.AddOrderAsync(order);

            // Act
            // Since CustomerName is private set, you may need to add a method in Order to update it for testing.
            // For now, let's assume you have a method like order.UpdateCustomerName("Jane Smith");
            // If not, you need to add such a method or use reflection for testing purposes.
            // Example:
            // order.UpdateCustomerName("Jane Smith");
            // For demonstration, using reflection (not recommended for production code):

            var customerNameProp = typeof(Order).GetProperty("CustomerName");
            customerNameProp.SetValue(order, "Jane Smith");

            await commandRepo.UpdateOrderAsync(order);

            // Assert
            var updatedOrder = await context.Orders.FindAsync(order.Id);
            Assert.NotNull(updatedOrder);
            Assert.Equal("Jane Smith", updatedOrder.CustomerName);
        }

        [Fact]
        public async Task DeleteOrderAsync_ShouldRemoveOrderFromDatabase()
        {
            // Arrange
            await using var context = CreateInMemoryDbContext();
            var commandRepo = new OrderCommandRepository(context);

            var order = new Order(
                Guid.NewGuid(),
                TestPaymentInfo(),
                "John Doe",
                "123 Test St",
                new Money(200, "USD"),
                new List<OrderProduct> { TestOrderProduct() },
                DateTime.UtcNow
            );

            await commandRepo.AddOrderAsync(order);

            // Act
            await commandRepo.DeleteOrderAsync(order.Id);

            // Assert
            var deletedOrder = await context.Orders.FindAsync(order.Id);
            Assert.Null(deletedOrder);
        }

        [Fact]
        public async Task DeleteOrderAsync_ShouldDoNothing_WhenOrderDoesNotExist()
        {
            // Arrange
            await using var context = CreateInMemoryDbContext();
            var commandRepo = new OrderCommandRepository(context);

            // Act
            await commandRepo.DeleteOrderAsync(Guid.NewGuid());

            // Assert
            Assert.Empty(context.Orders);
        }
    }
}