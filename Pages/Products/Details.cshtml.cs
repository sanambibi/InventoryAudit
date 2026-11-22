using System.Globalization;
using InventoryAudit.Models;
using InventoryAudit.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InventoryAudit.Pages.Products;

public class DetailsModel(InventoryService inventory) : PageModel
{
    public Product Product { get; private set; } = null!;
    public PriceStats Stats { get; private set; } = null!;
    public string Line { get; private set; } = "";
    public string Area { get; private set; } = "";
    public List<ChartDot> Dots { get; } = new();

    public const int ChartWidth = 640;
    public const int ChartHeight = 240;

    public IActionResult OnGet(int id)
    {
        var product = inventory.Get(id);
        if (product is null) return NotFound();

        Product = product;
        Stats = PriceCalculator.Analyze(product.HistoricalPrices);
        BuildChart();
        return Page();
    }

    private void BuildChart()
    {
        double[] p = Product.HistoricalPrices;
        const double pad = 36;
        double span = Stats.Range <= 0 ? 1 : Stats.Range;
        var points = new List<string>();
        double firstX = 0, lastX = 0;

        for (int i = 0; i < p.Length; i++)
        {
            double x = p.Length == 1 ? ChartWidth / 2.0 : pad + i * (ChartWidth - 2 * pad) / (p.Length - 1);
            double y = ChartHeight - pad - (p[i] - Stats.Min) / span * (ChartHeight - 2 * pad);
            if (i == 0) firstX = x;
            lastX = x;
            Dots.Add(new ChartDot(F(x), F(y), $"Price #{i + 1}: {Fmt.Money(p[i])}"));
            points.Add($"{F(x)},{F(y)}");
        }

        Line = string.Join(' ', points);
        Area = $"{F(firstX)},{F(ChartHeight - pad)} {Line} {F(lastX)},{F(ChartHeight - pad)}";
    }

    private static string F(double d) => d.ToString("0.##", CultureInfo.InvariantCulture);
}
