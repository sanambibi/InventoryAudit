using InventoryAudit.Exceptions;
using InventoryAudit.Models;

namespace InventoryAudit.Services;

/// <summary>Thread-safe in-memory product store.</summary>
public class InventoryService
{
    private readonly List<Product> _products = new();
    private readonly object _lock = new();
    private int _nextId = 1001;

    public InventoryService()
    {
        // Seed data (entered messy on purpose - the sanitizer cleans it).
        Add("  wireless   MOUSE pro ", "24.99, 22.50, 26.00, 25.75, 27.40");
        Add("LED desk LAMP", "39.90, 41.50, 38.00, 44.25, 42.10, 46.00");
        Add("  usb-c   charger 65W", "19.99, 21.50, 20.75, 18.99");
        Add("mechanical KEYBOARD rgb", "89.00, 94.50, 79.99, 84.00, 99.00");
        Add("smart   WATCH series 5", "199, 210, 205, 189.99, 215, 229");
        Add("hdmi cable 2m", "9.99, 10.49, 8.99, 9.49");
        Add(" 4K   uhd   Monitor 27inch ", "329, 349, 319.99, 309, 299.50");
        Add("bluetooth speaker   MINI", "34.00, 36.50, 29.99, 31.25, 33.00");
    }

    public IReadOnlyList<Product> All()
    {
        lock (_lock) return _products.ToList();
    }

    public Product? Get(int id)
    {
        lock (_lock) return _products.FirstOrDefault(p => p.ProductId == id);
    }

    public bool Exists(string cleanName)
    {
        lock (_lock) return _products.Any(p => p.Name.Equals(cleanName, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>Sanitizes + validates, then stores. Throws InvalidProductException / InvalidPriceException.</summary>
    public Product Add(string? rawName, string? rawPrices)
    {
        string clean = TitleSanitizer.Clean(rawName);
        double[] prices = PriceParser.Parse(rawPrices);

        lock (_lock)
        {
            if (_products.Any(p => p.Name.Equals(clean, StringComparison.OrdinalIgnoreCase)))
                throw new DuplicateProductException(clean);

            var product = new Product
            {
                ProductId = _nextId++,
                Name = clean,
                HistoricalPrices = prices
            };
            _products.Add(product);
            return product;
        }
    }

    public bool Remove(int id)
    {
        lock (_lock) return _products.RemoveAll(p => p.ProductId == id) > 0;
    }
}
