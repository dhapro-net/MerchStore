using MediatR;
using MerchStore.Application.ShoppingCart.Queries;
using MerchStore.Domain.ShoppingCart;
using MerchStore.Domain.ShoppingCart.Interfaces;

namespace MerchStore.Application.ShoppingCart.Handlers;

/// <summary>
/// Handles the GetCartQuery.
/// </summary>
public class GetCartQueryHandler : IRequestHandler<GetCartQuery, Cart>
{
    private readonly IShoppingCartQueryRepository _repository;

    public GetCartQueryHandler(IShoppingCartQueryRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<Cart> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetCartByIdAsync(request.CartId, cancellationToken);
    }
}