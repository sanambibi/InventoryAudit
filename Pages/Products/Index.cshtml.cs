using InventoryAudit.Models;
using InventoryAudit.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InventoryAudit.Pages.Products;

public class IndexModel(InventoryService inventory) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? Q { get; set; }

    [TempData]
    public string? Message { get; set; }

    public List<ProductStat> Items { get; private set; } = new();

    public void OnGet()
    {
        var query = inventory.All().AsEnumerable();

        if (!string.IsNullOrWhiteSpace(Q))
        {
            string term = Q.Trim();
            query = query.Where(p =>
                p.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                p.Sku.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        Items = query.Select(p => new ProductStat(p, PriceCalculator.Analyze(p.HistoricalPrices))).ToList();
    }

    public IActionResult OnPostDelete(int id)
    {
        var product = inventory.Get(id);
        if (product is not null && inventory.Remove(id))
            Message = $"{product.Name} was removed from the inventory.";

        return RedirectToPage();
    }
}
