namespace InventoryAudit.Exceptions;

/// <summary>
/// Thrown for text that is not a number, negative prices, zero prices,
/// non-finite values (NaN / Infinity) or unrealistic values.
/// </summary>
public class InvalidPriceException : Exception
{
    public string? Token { get; }

    public InvalidPriceException(string message, string? token = null) : base(message)
    {
        Token = token;
    }
}
