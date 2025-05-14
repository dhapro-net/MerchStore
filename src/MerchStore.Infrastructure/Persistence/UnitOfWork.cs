using MerchStore.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace MerchStore.Infrastructure.Persistence;

/// <summary>
/// The Unit of Work pattern provides a way to group multiple database operations
/// into a single transaction that either all succeed or all fail together.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private readonly ILogger<UnitOfWork> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
    /// </summary>
    /// <param name="context">The database context to use.</param>
    /// <param name="logger">The logger to use for logging operations.</param>
    public UnitOfWork(AppDbContext context, ILogger<UnitOfWork> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Saves all changes made in the context to the database.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The number of affected entities.</returns>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Saving changes to the database.");
        return await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Begins a new transaction.
    /// </summary>
    public async Task BeginTransactionAsync()
    {
        _logger.LogInformation("Starting a new database transaction.");
        await _context.Database.BeginTransactionAsync();
    }

    /// <summary>
    /// Commits all changes made in the current transaction.
    /// </summary>
    public async Task CommitTransactionAsync()
    {
        _logger.LogInformation("Committing the current database transaction.");
        await _context.Database.CommitTransactionAsync();
    }

    /// <summary>
    /// Rolls back all changes made in the current transaction.
    /// </summary>
    public async Task RollbackTransactionAsync()
    {
        _logger.LogWarning("Rolling back the current database transaction.");
        await _context.Database.RollbackTransactionAsync();
    }
}