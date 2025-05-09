using MediatR;
using MerchStore.Application.ShoppingCart.Commands;
using MerchStore.Domain.ShoppingCart.Interfaces;

namespace MerchStore.Application.ShoppingCart.Handlers;

/// <summary>
/// Handles the AddCartCommand.
/// </summary>
public class AddCartCommandHandler : IRequestHandler<AddCartCommand>
{
    private readonly IShoppingCartCommandRepository _repository;

    public AddCartCommandHandler(IShoppingCartCommandRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

   public async Task<Unit> Handle(AddCartCommand request, CancellationToken cancellationToken)
    {
        if (request.Cart == null)
            throw new ArgumentNullException(nameof(request.Cart));

        await _repository.AddAsync(request.Cart, cancellationToken); 
        return Unit.Value;
    }
}