using System;
using MediatR;
using MerchStore.Application.ShoppingCart.DTOs;

namespace MerchStore.Application.ShoppingCart.Queries
{
    public class GetCartQuery : IRequest<CartDto>
    {
        public Guid CartId { get; set; }
        public GetCartQuery(Guid cartId)
        {
            CartId = cartId;
        }
    }


}