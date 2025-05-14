using MerchStore.Domain.Interfaces;

namespace MerchStore.Application.Common.Interfaces;

/// <summary>
/// Interface for providing access to all repositories.
/// This acts as an abstraction over individual repositories.
/// </summary>
/// <remarks>
/// The Repository Manager pattern offers several benefits:
///
/// 1. Single Entry Point: Provides a unified interface to access all repositories,
///    reducing the need to inject multiple repositories into services/handlers.
///
/// 2. Dependency Inversion: The application layer depends on abstractions (this interface)
///    rather than concrete implementations, following the Dependency Inversion Principle.
///
/// 3. Transaction Management: Centralizes transaction handling through the UnitOfWork property,
///    ensuring atomic operations across multiple repositories.
///
/// 4. Testability: Makes it easy to mock repository access in unit tests by substituting
///    a test implementation of this interface.
///
/// 5. Separation of Concerns: Keeps repository access logic separate from business logic
///    in the application layer, adhering to Clean Architecture principles.
///
/// 6. Reduced Coupling: Application layer components (command/query handlers) aren't directly
///    coupled to infrastructure concerns like data access implementations.
///
/// 7. Consistency: Ensures a consistent approach to data access across the application.
/// </remarks>
public interface IRepositoryManager
{
    /// <summary>
    /// Gets the product query repository.
    /// </summary>
    /// <remarks>
    /// Provides access to product-specific read operations.
    /// </remarks>
    IProductQueryRepository ProductQueryRepository { get; }

    /// <summary>
    /// Gets the product command repository.
    /// </summary>
    /// <remarks>
    /// Provides access to product-specific write operations.
    /// </remarks>
    IProductCommandRepository ProductCommandRepository { get; }

    /// <summary>
    /// Gets the order query repository.
    /// </summary>
    /// <remarks>
    /// Provides access to order-specific read operations.
    /// </remarks>
    IOrderQueryRepository OrderQueryRepository { get; }

    /// <summary>
    /// Gets the order command repository.
    /// </summary>
    /// <remarks>
    /// Provides access to order-specific write operations.
    /// </remarks>
    IOrderCommandRepository OrderCommandRepository { get; }

    /// <summary>
    /// Gets the unit of work to commit transactions.
    /// </summary>
    /// <remarks>
    /// The Unit of Work pattern coordinates the work of multiple repositories by
    /// tracking changes made during a business transaction and persisting them in one operation.
    /// This is particularly important in CQRS architecture where commands may affect
    /// multiple aggregates that need to be saved atomically.
    /// </remarks>
    IUnitOfWork UnitOfWork { get; }
}