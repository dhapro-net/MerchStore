using FluentValidation;
using MerchStore.Application.ShoppingCart.Commands;

namespace MerchStore.Application.ShoppingCart.Validators
{
    public class AddItemToCartCommandValidator : AbstractValidator<AddItemToCartCommand>
    {
        public AddItemToCartCommandValidator()
        {
            RuleFor(x => x.CartId)
                .NotEqual(Guid.Empty)
                .WithMessage("Cart ID cannot be empty");

            RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage("Product ID is required");

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than zero");
        }
    }
}