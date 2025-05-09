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
        await _repository.AddAsync(request.Cart, request.CancellationToken);
        return Unit.Value;
    }
}