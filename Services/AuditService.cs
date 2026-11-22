using InventoryAudit.Exceptions;
using InventoryAudit.Models;

namespace InventoryAudit.Services;

/// <summary>
/// Audits a batch of raw lines ("Product name | 10, 11, 12").
/// Every line is wrapped in try/catch so one bad entry never crashes the whole audit.
/// </summary>
public class AuditService(InventoryService inventory)
{
    public const string SampleBatch =
        "   wireless   MOUSE pro  | 24.99, 22.50, 26.00, 25.75\n" +
        "gaming   headset   v2 | 59, 62, 57.5, 64\n" +
        "  usb-c   charger 65W |19.99, abc, 21.5\n" +
        "gaming KEYBOARD rgb | 59, 62, -10, 64\n" +
        "   | 10, 12\n" +
        "mechanical   Switch Tester |\n" +
        "smart   WATCH series 5 | 199, 210, 205, 189.99, 215\n" +
        "hdmi CABLE 3m | 9.99, 1e9\n" +
        "12345 | 5, 6\n" +
        "webcam full hd 1080p   9.99, 10.5";

    public AuditReport Run(string? batch, bool commit)
    {
        var report = new AuditReport();
        if (string.IsNullOrWhiteSpace(batch)) return report;

        var seenInBatch = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        string[] lines = batch.Replace("\r", "").Split('\n');

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            if (string.IsNullOrWhiteSpace(line)) continue;

            var entry = new AuditEntry { LineNo = i + 1, RawName = line };

            try
            {
                int bar = line.IndexOf('|');
                if (bar < 0)
                    throw new FormatException("Missing the '|' separator. Use: Product Name | 10.5, 11, 12");

                entry.RawName = line[..bar];
                entry.RawPrices = line[(bar + 1)..];

                entry.CleanName = TitleSanitizer.Clean(entry.RawName);
                double[] prices = PriceParser.Parse(entry.RawPrices);
                entry.Stats = PriceCalculator.Analyze(prices);

                if (!seenInBatch.Add(entry.CleanName))
                    throw new DuplicateProductException(entry.CleanName);

                if (commit)
                {
                    inventory.Add(entry.RawName, entry.RawPrices);
                    entry.Committed = true;
                }
                else if (inventory.Exists(entry.CleanName))
                {
                    throw new DuplicateProductException(entry.CleanName);
                }

                entry.Success = true;
            }
            catch (DuplicateProductException ex)
            {
                entry.Error = ex.Message; entry.ErrorType = "Duplicate";
            }
            catch (InvalidProductException ex)
            {
                entry.Error = ex.Message; entry.ErrorType = "Invalid name";
            }
            catch (InvalidPriceException ex)
            {
                entry.Error = ex.Message; entry.ErrorType = "Invalid price";
            }
            catch (FormatException ex)
            {
                entry.Error = ex.Message; entry.ErrorType = "Bad format";
            }
            catch (Exception ex)
            {
                entry.Error = "Unexpected problem: " + ex.Message; entry.ErrorType = "Unexpected";
            }

            report.Entries.Add(entry);
        }

        return report;
    }
}
