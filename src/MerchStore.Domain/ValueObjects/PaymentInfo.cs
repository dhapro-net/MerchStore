namespace MerchStore.Domain.ValueObjects;

public class PaymentInfo
{
    private const int CardNumberLength = 16;
    private const int ExpirationDateLength = 5;
    private const int CVVLength = 3;

    public string CardNumber { get; private set; } = null!;
    public string ExpirationDate { get; private set; } = null!;
    public string CVV { get; private set; } = null!;

    private PaymentInfo() { }

    // Add this constructor for test support
    internal PaymentInfo(string cardNumber, string cvv, DateTime expiration)
    {
        CardNumber = cardNumber;
        CVV = cvv;
        ExpirationDate = expiration.ToString("MM/yy");
    }

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

        // Strictly require MM/yy format using regex
        if (string.IsNullOrWhiteSpace(expirationDate)
            || expirationDate.Length != ExpirationDateLength
            || !System.Text.RegularExpressions.Regex.IsMatch(expirationDate, @"^(0[1-9]|1[0-2])\/\d{2}$")
            || !IsValidExpirationDate(expirationDate))
            throw new ArgumentException($"Expiration date must be in MM/YY format and in the future.", nameof(expirationDate));

        if (string.IsNullOrWhiteSpace(cvv) || cvv.Length != CVVLength || !cvv.All(char.IsDigit))
            throw new ArgumentException($"CVV must be {CVVLength} digits and contain only numeric characters.", nameof(cvv));

        CardNumber = cardNumber;
        ExpirationDate = expirationDate;
        CVV = cvv;
    }
    private static bool IsValidExpirationDate(string expirationDate)
    {
        // Parse MM/yy format
        if (!DateTime.TryParseExact(expirationDate, "MM/yy", null, System.Globalization.DateTimeStyles.None, out var parsedDate))
            return false;

        // Always treat 2-digit years as 2000+
        if (parsedDate.Year < 100)
            parsedDate = new DateTime(2000 + parsedDate.Year, parsedDate.Month, 1);

        // Card is valid through the last day of the expiration month
        var lastDayOfMonth = new DateTime(parsedDate.Year, parsedDate.Month, DateTime.DaysInMonth(parsedDate.Year, parsedDate.Month));
        return lastDayOfMonth >= DateTime.UtcNow.Date;
    }
}