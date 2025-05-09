using System;
using MediatR;
using MerchStore.Application.Common;

namespace MerchStore.Application.ShoppingCart.Commands
{
    public class RemoveProductFromCartCommand : IRequest<Result<bool>>
    {
        public Guid CartId { get; set; }
        public string ProductId { get; set; }

        public CancellationToken CancellationToken {get;}
        public RemoveProductFromCartCommand(Guid cartId, string productId, CancellationToken cancellationToken)
        {
            CartId = cartId;
            ProductId = productId;
            CancellationToken = cancellationToken;
    }
}
}