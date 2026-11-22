using InventoryAudit.Exceptions;
using InventoryAudit.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InventoryAudit.Pages.Products;

public class CreateModel(InventoryService inventory) : PageModel
{
    [BindProperty] public string? Name { get; set; }
    [BindProperty] public string? Prices { get; set; }

    public string? ErrorTitle { get; private set; }
    public string? ErrorText { get; private set; }

    public void OnGet() { }

    public IActionResult OnPost()
    {
        try
        {
            var product = inventory.Add(Name, Prices);
            TempData["Success"] = $"\"{product.Name}\" was added as {product.Sku}.";
            return RedirectToPage("Details", new { id = product.ProductId });
        }
        catch (DuplicateProductException ex)
        {
            ErrorTitle = "Duplicate product"; ErrorText = ex.Message;
        }
        catch (InvalidProductException ex)
        {
            ErrorTitle = "Check the product name"; ErrorText = ex.Message;
        }
        catch (InvalidPriceException ex)
        {
            ErrorTitle = "Check the prices"; ErrorText = ex.Message;
        }
        catch (Exception ex)
        {
            ErrorTitle = "Unexpected problem"; ErrorText = ex.Message;
        }

        return Page();
    }
}
