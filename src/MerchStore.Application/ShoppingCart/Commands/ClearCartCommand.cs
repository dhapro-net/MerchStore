using System;
using MediatR;
using MerchStore.Application.Common;

namespace MerchStore.Application.ShoppingCart.Commands
{
    public class ClearCartCommand : IRequest<Result<bool>>
    {
        public Guid CartId { get; set; }
    }
}