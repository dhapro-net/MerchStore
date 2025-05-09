using FluentValidation;
using MerchStore.Application.ShoppingCart.Commands;

namespace MerchStore.Application.ShoppingCart.Validators
{
    public class AddProductToCartCommandValidator : AbstractValidator<AddProductToCartCommand>
    {
        public AddProductToCartCommandValidator()
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