using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MerchStore.Domain.Entities;
using MerchStore.Infrastructure.Persistence;
using MerchStore.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace MerchStore.Tests.Infrastructure.Repositories
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

        [Fact]
        public async Task AddOrderAsync_ShouldAddOrderToDatabase()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            var repository = new OrderRepository(context);

            var order = new Order
            {
                Id = Guid.NewGuid(),
                CustomerName = "John Doe",
                CreatedDate = DateTime.UtcNow,
                Items = new List<OrderItem>
                {
                    new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        ProductId = Guid.NewGuid(),
                        ProductName = "Test Product",
                        Price = new Money(100, "USD"),
                        Quantity = 2
                    }
                }
            };

            // Act
            var result = await repository.AddOrderAsync(order);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(order.Id, result.Id);
            Assert.Single(context.Orders);
        }

        [Fact]
        public async Task GetOrderByIdAsync_ShouldReturnOrder_WhenOrderExists()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            var repository = new OrderRepository(context);

            var order = new Order
            {
                Id = Guid.NewGuid(),
                CustomerName = "John Doe",
                CreatedDate = DateTime.UtcNow
            };

            context.Orders.Add(order);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetOrderByIdAsync(order.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(order.Id, result.Id);
        }

        [Fact]
        public async Task GetOrderByIdAsync_ShouldReturnNull_WhenOrderDoesNotExist()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            var repository = new OrderRepository(context);

            // Act
            var result = await repository.GetOrderByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetOrdersByCustomerAsync_ShouldReturnOrdersForCustomer()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            var repository = new OrderRepository(context);

            var customerId = Guid.NewGuid();
            var orders = new List<Order>
            {
                new Order { Id = Guid.NewGuid(), CustomerId = customerId, CustomerName = "John Doe", CreatedDate = DateTime.UtcNow },
                new Order { Id = Guid.NewGuid(), CustomerId = customerId, CustomerName = "John Doe", CreatedDate = DateTime.UtcNow },
                new Order { Id = Guid.NewGuid(), CustomerId = Guid.NewGuid(), CustomerName = "Jane Smith", CreatedDate = DateTime.UtcNow }
            };

            context.Orders.AddRange(orders);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetOrdersByCustomerAsync(customerId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetOrdersByCustomerAsync_ShouldReturnEmpty_WhenNoOrdersExist()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            var repository = new OrderRepository(context);

            // Act
            var result = await repository.GetOrdersByCustomerAsync(Guid.NewGuid());

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetOrdersByDateRangeAsync_ShouldReturnOrdersWithinDateRange()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            var repository = new OrderRepository(context);

            var orders = new List<Order>
            {
                new Order { Id = Guid.NewGuid(), CreatedDate = DateTime.UtcNow.AddDays(-5) },
                new Order { Id = Guid.NewGuid(), CreatedDate = DateTime.UtcNow.AddDays(-3) },
                new Order { Id = Guid.NewGuid(), CreatedDate = DateTime.UtcNow.AddDays(-1) }
            };

            context.Orders.AddRange(orders);
            await context.SaveChangesAsync();

            var startDate = DateTime.UtcNow.AddDays(-4);
            var endDate = DateTime.UtcNow;

            // Act
            var result = await repository.GetOrdersByDateRangeAsync(startDate, endDate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetOrdersByDateRangeAsync_ShouldIncludeOrdersOnStartAndEndDate()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            var repository = new OrderRepository(context);

            var startDate = DateTime.UtcNow.AddDays(-5);
            var endDate = DateTime.UtcNow;

            var orders = new List<Order>
            {
                new Order { Id = Guid.NewGuid(), CreatedDate = startDate },
                new Order { Id = Guid.NewGuid(), CreatedDate = endDate },
                new Order { Id = Guid.NewGuid(), CreatedDate = DateTime.UtcNow.AddDays(-3) }
            };

            context.Orders.AddRange(orders);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetOrdersByDateRangeAsync(startDate, endDate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task UpdateOrderAsync_ShouldUpdateOrderInDatabase()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            var repository = new OrderRepository(context);

            var order = new Order
            {
                Id = Guid.NewGuid(),
                CustomerName = "John Doe",
                CreatedDate = DateTime.UtcNow
            };

            context.Orders.Add(order);
            await context.SaveChangesAsync();

            // Act
            order.CustomerName = "Jane Smith";
            await repository.UpdateOrderAsync(order);

            // Assert
            var updatedOrder = await context.Orders.FindAsync(order.Id);
            Assert.NotNull(updatedOrder);
            Assert.Equal("Jane Smith", updatedOrder.CustomerName);
        }

        [Fact]
        public async Task DeleteOrderAsync_ShouldRemoveOrderFromDatabase()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            var repository = new OrderRepository(context);

            var order = new Order
            {
                Id = Guid.NewGuid(),
                CustomerName = "John Doe",
                CreatedDate = DateTime.UtcNow
            };

            context.Orders.Add(order);
            await context.SaveChangesAsync();

            // Act
            await repository.DeleteOrderAsync(order.Id);

            // Assert
            var deletedOrder = await context.Orders.FindAsync(order.Id);
            Assert.Null(deletedOrder);
        }

        [Fact]
        public async Task DeleteOrderAsync_ShouldDoNothing_WhenOrderDoesNotExist()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            var repository = new OrderRepository(context);

            // Act
            await repository.DeleteOrderAsync(Guid.NewGuid());

            // Assert
            Assert.Empty(context.Orders);
        }
    }
}