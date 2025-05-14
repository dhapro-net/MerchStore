using MerchStore.Application.Common.Interfaces;
using MerchStore.Domain.Interfaces;

namespace MerchStore.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementation of the Repository Manager pattern.
/// This class provides a single point of access to all repositories and the unit of work.
/// </summary>
public class RepositoryManager : IRepositoryManager
{
    private readonly IProductQueryRepository _productQueryRepository;
    private readonly IProductCommandRepository _productCommandRepository;
    private readonly IOrderQueryRepository _orderQueryRepository;
    private readonly IOrderCommandRepository _orderCommandRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Constructor that accepts all required repositories and the unit of work.
    /// </summary>
    /// <param name="productQueryRepository">The product query repository.</param>
    /// <param name="productCommandRepository">The product command repository.</param>
    /// <param name="orderQueryRepository">The order query repository.</param>
    /// <param name="orderCommandRepository">The order command repository.</param>
    /// <param name="unitOfWork">The unit of work.</param>
    public RepositoryManager(
        IProductQueryRepository productQueryRepository,
        IProductCommandRepository productCommandRepository,
        IOrderQueryRepository orderQueryRepository,
        IOrderCommandRepository orderCommandRepository,
        IUnitOfWork unitOfWork)
    {
        _productQueryRepository = productQueryRepository ?? throw new ArgumentNullException(nameof(productQueryRepository));
        _productCommandRepository = productCommandRepository ?? throw new ArgumentNullException(nameof(productCommandRepository));
        _orderQueryRepository = orderQueryRepository ?? throw new ArgumentNullException(nameof(orderQueryRepository));
        _orderCommandRepository = orderCommandRepository ?? throw new ArgumentNullException(nameof(orderCommandRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    /// <inheritdoc/>
    public IProductQueryRepository ProductQueryRepository => _productQueryRepository;

    /// <inheritdoc/>
    public IProductCommandRepository ProductCommandRepository => _productCommandRepository;

    /// <inheritdoc/>
    public IOrderQueryRepository OrderQueryRepository => _orderQueryRepository;

    /// <inheritdoc/>
    public IOrderCommandRepository OrderCommandRepository => _orderCommandRepository;

    /// <inheritdoc/>
    public IUnitOfWork UnitOfWork => _unitOfWork;
}