namespace InventoryAudit.Models;

public class AuditEntry
{
    public int LineNo { get; set; }
    public string RawName { get; set; } = string.Empty;
    public string RawPrices { get; set; } = string.Empty;
    public string? CleanName { get; set; }
    public PriceStats? Stats { get; set; }
    public bool Success { get; set; }
    public bool Committed { get; set; }
    public string? Error { get; set; }
    public string? ErrorType { get; set; }
}

public class AuditReport
{
    public List<AuditEntry> Entries { get; } = new();
    public int Accepted => Entries.Count(e => e.Success);
    public int Rejected => Entries.Count(e => !e.Success);
    public int Committed => Entries.Count(e => e.Committed);
}

public record SanitizeStep(string Method, string Value);
public record ChartDot(string X, string Y, string Label);
