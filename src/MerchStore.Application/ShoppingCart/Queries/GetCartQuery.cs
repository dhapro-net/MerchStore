using System;
using MediatR;
using MerchStore.Application.ShoppingCart.Dtos;

namespace MerchStore.Application.ShoppingCart.Queries
{
    public class GetCartQuery : IRequest<CartDto>
    {
        public Guid CartId { get; set; }
    }
}