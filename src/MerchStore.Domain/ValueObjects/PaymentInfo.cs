namespace MerchStore.Domain.ValueObjects;

public class PaymentInfo
{
    private const int CardNumberLength = 16;
    private const int ExpirationDateLength = 5;
    private const int CVVLength = 3;

    public string CardNumber { get; private set; }
    public string ExpirationDate { get; private set; }
    public string CVV { get; private set; }

    private PaymentInfo() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PaymentInfo"/> class.
    /// </summary>
    /// <param name="cardNumber">The card number (16 digits).</param>
    /// <param name="expirationDate">The expiration date in MM/YY format.</param>
    /// <param name="cvv">The CVV (3 digits).</param>
    /// <exception cref="ArgumentException">Thrown when any parameter is invalid.</exception>
    public PaymentInfo(string cardNumber, string expirationDate, string cvv)
    {
        if (string.IsNullOrWhiteSpace(cardNumber) || cardNumber.Length != CardNumberLength || !cardNumber.All(char.IsDigit))
            throw new ArgumentException($"Card number must be {CardNumberLength} digits and contain only numeric characters.", nameof(cardNumber));

        if (string.IsNullOrWhiteSpace(expirationDate) || expirationDate.Length != ExpirationDateLength || !IsValidExpirationDate(expirationDate))
            throw new ArgumentException($"Expiration date must be in MM/YY format and in the future.", nameof(expirationDate));

        if (string.IsNullOrWhiteSpace(cvv) || cvv.Length != CVVLength || !cvv.All(char.IsDigit))
            throw new ArgumentException($"CVV must be {CVVLength} digits and contain only numeric characters.", nameof(cvv));

        CardNumber = cardNumber;
        ExpirationDate = expirationDate;
        CVV = cvv;
    }

    private static bool IsValidExpirationDate(string expirationDate)
    {
        if (!DateTime.TryParseExact(expirationDate, "MM/yy", null, System.Globalization.DateTimeStyles.None, out var parsedDate))
            return false;

        // Set the expiration date to the last day of the month
        parsedDate = parsedDate.AddMonths(1).AddDays(-1);

        return parsedDate >= DateTime.UtcNow;
    }
}