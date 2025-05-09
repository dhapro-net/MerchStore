using System;
using MediatR;
using MerchStore.Application.Common;

namespace MerchStore.Application.ShoppingCart.Commands
{
    public class UpdateCartProductQuantityCommand : IRequest<Result<bool>>
    {
        public Guid CartId { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; }
    
            public UpdateCartProductQuantityCommand(Guid cartId, string productId, int quantity)
        {
            CartId = cartId;
            ProductId = productId;
            Quantity = quantity;
        }
    }
}