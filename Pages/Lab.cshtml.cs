using Microsoft.AspNetCore.Mvc;
using InventoryAudit.Exceptions;
using InventoryAudit.Models;
using InventoryAudit.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InventoryAudit.Pages;

public class LabModel : PageModel
{
    private const string DefaultTitle = "   mechanical   KEYBOARD  rgb ";
    private const string DefaultPrices = "59, 62, 57.5, 64";

    [BindProperty] public string? RawTitle { get; set; }
    [BindProperty] public string? RawPrices { get; set; }

    public List<SanitizeStep>? Steps { get; private set; }
    public string? TitleError { get; private set; }
    public double[]? Parsed { get; private set; }
    public PriceStats? Stats { get; private set; }
    public string? PriceError { get; private set; }

    public void OnGet()
    {
        RawTitle = DefaultTitle;
        RawPrices = DefaultPrices;
    }

    public void OnPostTitle()
    {
        RawPrices ??= DefaultPrices;
        try
        {
            Steps = TitleSanitizer.Explain(RawTitle);
        }
        catch (InvalidProductException ex)
        {
            TitleError = ex.Message;
        }
    }

    public void OnPostPrices()
    {
        RawTitle ??= DefaultTitle;
        try
        {
            Parsed = PriceParser.Parse(RawPrices);
            Stats = PriceCalculator.Analyze(Parsed);
        }
        catch (InvalidPriceException ex)
        {
            PriceError = ex.Message;
        }
    }
}
