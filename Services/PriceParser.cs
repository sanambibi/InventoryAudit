using System.Globalization;
using InventoryAudit.Exceptions;

namespace InventoryAudit.Services;

/// <summary>Turns text like "24.99, 22.5; 26" into a validated double[].</summary>
public static class PriceParser
{
    private static readonly char[] Separators = { ',', ';', '\n', '\r' };

    public static double[] Parse(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            throw new InvalidPriceException("No prices were entered. Example: 24.99, 22.50, 26.00");

        string[] tokens = raw.Split(Separators,
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (tokens.Length == 0)
            throw new InvalidPriceException("No prices were entered. Example: 24.99, 22.50, 26.00");

        var result = new double[tokens.Length];

        for (int i = 0; i < tokens.Length; i++)
        {
            string token = tokens[i].TrimStart('$').Trim();

            try
            {
                result[i] = double.Parse(token, NumberStyles.Float, CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                throw new InvalidPriceException(
                    $"Price #{i + 1} \"{tokens[i]}\" is not a valid number. Use digits and a dot, e.g. 19.99.", tokens[i]);
            }
            catch (OverflowException)
            {
                throw new InvalidPriceException($"Price #{i + 1} \"{tokens[i]}\" is too large to store.", tokens[i]);
            }

            PriceCalculator.ValidatePrice(result[i], i + 1);
        }

        return result;
    }
}
