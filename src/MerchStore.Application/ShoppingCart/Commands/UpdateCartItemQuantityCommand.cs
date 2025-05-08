using System;
using MediatR;
using MerchStore.Application.Common;

namespace MerchStore.Application.ShoppingCart.Commands
{
    public class UpdateCartItemQuantityCommand : IRequest<Result<bool>>
    {
        public Guid CartId { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; }
    
            public UpdateCartItemQuantityCommand(Guid cartId, string productId, int quantity)
        {
            CartId = cartId;
            ProductId = productId;
            Quantity = quantity;
        }
    }
}