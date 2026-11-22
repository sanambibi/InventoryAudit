using System.Globalization;
using System.Text.RegularExpressions;
using InventoryAudit.Exceptions;
using InventoryAudit.Models;

namespace InventoryAudit.Services;

/// <summary>
/// String sanitization for product titles using built-in string functions:
/// Trim(), Split(), Join(), ToLowerInvariant() and TextInfo.ToTitleCase().
/// </summary>
public static class TitleSanitizer
{
    private const int MinLength = 2;
    private const int MaxLength = 80;

    // Words that must stay UPPERCASE after title-casing (LED, USB, HDMI ...)
    private static readonly HashSet<string> Acronyms = new(StringComparer.OrdinalIgnoreCase)
    {
        "LED", "USB", "HDMI", "RGB", "SSD", "HDD", "TV", "HD", "UHD", "LCD",
        "GPS", "RAM", "CPU", "GPU", "LTE", "NFC", "UV", "WIFI", "DVD"
    };

    /// <summary>Cleans a raw title or throws <see cref="InvalidProductException"/>.</summary>
    public static string Clean(string? raw)
    {
        var steps = Explain(raw);
        return steps[^1].Value;
    }

    /// <summary>Same as Clean, but returns every transformation step (used by the Lab page).</summary>
    public static List<SanitizeStep> Explain(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            throw new InvalidProductException("Product name is empty. Type at least a couple of characters.");

        var steps = new List<SanitizeStep> { new("Original input", raw) };

        // 1. remove leading / trailing spaces
        string trimmed = raw.Trim();
        steps.Add(new("Trim()", trimmed));

        // 2. collapse repeated inner spaces / tabs into one single space
        string[] words = trimmed.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
        string collapsed = string.Join(' ', words);
        steps.Add(new("Split() + Join(' ')", collapsed));

        // 3. normalise the casing
        string lowered = collapsed.ToLowerInvariant();
        steps.Add(new("ToLowerInvariant()", lowered));

        string titled = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(lowered);
        steps.Add(new("ToTitleCase()", titled));

        // 4. keep known acronyms uppercase
        string final = Regex.Replace(titled, "[A-Za-z]+",
            m => Acronyms.Contains(m.Value) ? m.Value.ToUpperInvariant() : m.Value);
        steps.Add(new("Acronym fix (LED, USB, HDMI...)", final));

        // 5. validate the final result
        if (final.Length < MinLength)
            throw new InvalidProductException($"Product name \"{final}\" is too short (minimum {MinLength} characters).");

        if (final.Length > MaxLength)
            throw new InvalidProductException($"Product name is too long ({final.Length} characters, maximum {MaxLength}).");

        if (!final.Any(char.IsLetter))
            throw new InvalidProductException($"Product name \"{final}\" must contain at least one letter.");

        if (final.Any(char.IsControl))
            throw new InvalidProductException("Product name contains hidden control characters.");

        return steps;
    }
}
