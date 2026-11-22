# StockLedger - Smart Inventory & Product Price Auditing

ASP.NET Core Razor Pages web app on **.NET 10** (C#) for a retail management tool.

## Run
```
dotnet run
```
Then open http://localhost:5050 (or the URL printed in the console).
Requires the .NET 10 SDK: https://dotnet.microsoft.com/download

## Pages
| Page | What it does |
|---|---|
| Dashboard | Totals, unstable prices, latest products, animated title clean-up demo |
| Products | List with search, min / max / average / latest price, remove |
| Add product | Messy name + price list, live preview, clear error messages |
| Product details | Animated price chart + full price table |
| Batch audit | Paste many lines, every line audited in its own try/catch |
| Clean-up lab | Step-by-step title sanitizer and price calculator |
| How it works | Where each requirement lives in the code |

## Requirement map
- Product class (name, id, `double[]` prices): `Models/Product.cs`
- String sanitization (`Trim`, `Split`, `Join`, `ToLowerInvariant`, `ToTitleCase`): `Services/TitleSanitizer.cs`
- Max / min / average: `Services/PriceCalculator.cs`
- try / catch + meaningful exceptions: `Services/PriceParser.cs`, `Exceptions/`, `Services/AuditService.cs`

## Themes
Click the four colour dots in the header: Midnight, Violet, Emerald, Crimson. The choice is remembered.

## Notes
Data is stored in memory (no database). Restarting restores the sample products.
Enter prices without thousand separators (a comma separates prices).
