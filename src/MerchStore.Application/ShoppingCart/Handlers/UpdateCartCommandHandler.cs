using MediatR;
using MerchStore.Application.ShoppingCart.Commands;
using MerchStore.Domain.ShoppingCart.Interfaces;

namespace MerchStore.Application.ShoppingCart.Handlers;

/// <summary>
/// Handles the UpdateCartCommand.
/// </summary>
public class UpdateCartCommandHandler : IRequestHandler<UpdateCartCommand>
{
    private readonly IShoppingCartCommandRepository _repository;

    public UpdateCartCommandHandler(IShoppingCartCommandRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<Unit> Handle(UpdateCartCommand request, CancellationToken cancellationToken)
    {
        await _repository.UpdateAsync(request.Cart);
        return Unit.Value;
    }
}