using MerchStore.Application.Models.Checkout;
using System.Threading.Tasks;

namespace MerchStore.Application.Services.Interfaces
{
    public interface ICheckoutService
    {

        Task ProcessCheckoutAsync(CheckoutRequest checkoutRequest);
    }
}