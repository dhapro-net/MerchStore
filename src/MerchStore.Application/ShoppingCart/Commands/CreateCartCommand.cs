

using MediatR;
using MerchStore.Application.ShoppingCart.DTOs;

namespace MerchStore.Application.ShoppingCart.Commands
{
    public class CreateCartCommand : IRequest<CartDto>
    {
        /// <summary>
        /// Gets the ID of the cart to be created.
        /// </summary>
        public Guid CartId { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateCartCommand"/> class.
        /// </summary>
        /// <param name="cartId">The unique identifier for the cart.</param>
        public CreateCartCommand(Guid cartId)
        {
            CartId = cartId;
        }
    }
}