using System;
using MediatR;
using MerchStore.Application.Common;

namespace MerchStore.Application.ShoppingCart.Commands
{
    public class RemoveItemFromCartCommand : IRequest<Result<bool>>
    {
        public Guid CartId { get; set; }
        public string ProductId { get; set; }
        public RemoveItemFromCartCommand(Guid cartId, string productId)
        {
            CartId = cartId;
            ProductId = productId;
        }
    }
}