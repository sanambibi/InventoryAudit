using InventoryAudit.Models;
using InventoryAudit.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InventoryAudit.Pages;

public class IndexModel(InventoryService inventory) : PageModel
{
    public List<ProductStat> Items { get; private set; } = new();
    public int TotalProducts => Items.Count;
    public int TotalPricePoints => Items.Sum(i => i.Stats.Count);
    public double AverageOfAverages => Items.Count == 0 ? 0 : Items.Average(i => i.Stats.Average);
    public ProductStat? MostExpensive => Items.OrderByDescending(i => i.Stats.Max).FirstOrDefault();
    public List<ProductStat> BiggestSwings { get; private set; } = new();
    public List<ProductStat> Recent { get; private set; } = new();

    public void OnGet()
    {
        Items = inventory.All()
            .Select(p => new ProductStat(p, PriceCalculator.Analyze(p.HistoricalPrices)))
            .ToList();

        BiggestSwings = Items.OrderByDescending(i => i.Stats.Range / i.Stats.Average).Take(5).ToList();
        Recent = Items.OrderByDescending(i => i.Product.ProductId).Take(4).ToList();
    }
}
