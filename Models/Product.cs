namespace InventoryAudit.Models;

/// <summary>
/// A product in the retail inventory: name, id and an array of historical prices.
/// </summary>
public class Product
{
    public int ProductId { get; init; }
    public string Name { get; set; } = string.Empty;
    public double[] HistoricalPrices { get; set; } = Array.Empty<double>();
    public DateTime AddedAt { get; init; } = DateTime.Now;

    public string Sku => $"SKU-{ProductId:D4}";
}
