namespace InventoryAudit.Models;

/// <summary>Result of the price calculation (min / max / average + helpers).</summary>
public record PriceStats(double Min, double Max, double Average, double First, double Latest, int Count)
{
    public double Range => Max - Min;

    /// <summary>% change from the first recorded price to the latest one.</summary>
    public double TrendPercent => First == 0 ? 0 : (Latest - First) / First * 100.0;
}

public record ProductStat(Product Product, PriceStats Stats);
