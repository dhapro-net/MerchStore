using MediatR;
using MerchStore.Application.ShoppingCart.Interfaces;
using MerchStore.Domain.ShoppingCart.Interfaces;
using MerchStore.Domain.ValueObjects;

namespace MerchStore.Application.ShoppingCart.Queries
{
    public class CalculateCartTotalQueryHandler : IRequestHandler<CalculateCartTotalQuery, Money>
    {
        private readonly IShoppingCartRepository _repository;

        public CalculateCartTotalQueryHandler(IShoppingCartRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<Money> Handle(CalculateCartTotalQuery request, CancellationToken cancellationToken)
        {
            var cart = await _repository.GetCartByIdAsync(request.CartId, cancellationToken);
            if (cart == null)
            {
                throw new InvalidOperationException($"Cart with ID {request.CartId} not found.");
            }

            // Assuming the cart has a CalculateTotal method that returns Money
            return cart.CalculateTotal();
        }
    }
}