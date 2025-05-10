namespace MerchStore.Domain.ValueObjects;

public class PaymentInfo
{
    public string CardNumber { get; private set; }
    public string ExpirationDate { get; private set; }
    public string CVV { get; private set; }

    private PaymentInfo() { }

    // Constructor to ensure immutability
    public PaymentInfo(string cardNumber, string expirationDate, string cvv)
    {
        if (string.IsNullOrWhiteSpace(cardNumber) || cardNumber.Length != 16)
            throw new ArgumentException("Card number must be 16 digits.");
        if (string.IsNullOrWhiteSpace(expirationDate) || expirationDate.Length != 5)
            throw new ArgumentException("Expiration date must be in MM/YY format.");
        if (string.IsNullOrWhiteSpace(cvv) || cvv.Length != 3)
            throw new ArgumentException("CVV must be 3 digits.");

        CardNumber = cardNumber;
        ExpirationDate = expirationDate;
        CVV = cvv;
    }
}