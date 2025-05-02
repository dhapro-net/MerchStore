public class CheckoutService : ICheckoutService
{
    private readonly IOrderRepository _orderRepository;

    public CheckoutService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task ProcessCheckoutAsync(CheckoutRequest checkoutRequest)
    {
        // Map the checkout request to an order entity
        var order = new Order
        {
            Items = checkoutRequest.Items,
            Shipping = checkoutRequest.Shipping,
            Payment = checkoutRequest.Payment,
            TotalPrice = checkoutRequest.TotalPrice,
            OrderDate = DateTime.UtcNow
        };

        // Save the order to the database
        await _orderRepository.SaveOrderAsync(order);
    }
}