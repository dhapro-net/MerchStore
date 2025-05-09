using MediatR;
using MerchStore.Application.ShoppingCart.Commands;
using MerchStore.Domain.ShoppingCart.Interfaces;

namespace MerchStore.Application.ShoppingCart.Handlers;

/// <summary>
/// Handles the DeleteCartCommand.
/// </summary>
public class DeleteCartCommandHandler : IRequestHandler<DeleteCartCommand>
{
    private readonly IShoppingCartCommandRepository _repository;

    public DeleteCartCommandHandler(IShoppingCartCommandRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<Unit> Handle(DeleteCartCommand request, CancellationToken cancellationToken)
    {
        await _repository.DeleteAsync(request.CartId);
        return Unit.Value;
    }
}