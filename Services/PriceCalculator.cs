using System.Globalization;
using InventoryAudit.Exceptions;
using InventoryAudit.Models;

namespace InventoryAudit.Services;

/// <summary>Maximum, minimum and average price calculation over a double[] array.</summary>
public static class PriceCalculator
{
    public const double MaxReasonablePrice = 1_000_000;

    /// <summary>Validates one price. Throws <see cref="InvalidPriceException"/> with a meaningful message.</summary>
    public static void ValidatePrice(double value, int position)
    {
        string shown = value.ToString("0.##", CultureInfo.InvariantCulture);

        if (!double.IsFinite(value))
            throw new InvalidPriceException($"Price #{position} is not a finite number (NaN / Infinity).", shown);

        if (value < 0)
            throw new InvalidPriceException($"Price #{position} is negative ({shown}). Prices cannot be below zero.", shown);

        if (value == 0)
            throw new InvalidPriceException($"Price #{position} is zero. A sellable product needs a price above zero.", shown);

        if (value > MaxReasonablePrice)
            throw new InvalidPriceException(
                $"Price #{position} ({shown}) is unrealistically high (limit {MaxReasonablePrice:N0}). Possible typo?", shown);
    }

    public static void Validate(double[]? prices)
    {
        if (prices is null || prices.Length == 0)
            throw new InvalidPriceException("Price history is empty. Add at least one price.");

        for (int i = 0; i < prices.Length; i++)
            ValidatePrice(prices[i], i + 1);
    }

    public static double Max(double[] prices)
    {
        Validate(prices);
        double max = prices[0];
        foreach (double p in prices)
            if (p > max) max = p;
        return max;
    }

    public static double Min(double[] prices)
    {
        Validate(prices);
        double min = prices[0];
        foreach (double p in prices)
            if (p < min) min = p;
        return min;
    }

    public static double Average(double[] prices)
    {
        Validate(prices);
        double sum = 0;
        foreach (double p in prices)
            sum += p;
        return sum / prices.Length;
    }

    public static PriceStats Analyze(double[] prices)
    {
        Validate(prices);
        return new PriceStats(
            Min: Min(prices),
            Max: Max(prices),
            Average: Average(prices),
            First: prices[0],
            Latest: prices[^1],
            Count: prices.Length);
    }
}
