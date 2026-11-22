using System.Globalization;

namespace InventoryAudit.Services;

public static class Fmt
{
    public static string Money(double v) => "$" + v.ToString("N2", CultureInfo.InvariantCulture);

    public static string Percent(double v) =>
        (v >= 0 ? "+" : "") + v.ToString("0.0", CultureInfo.InvariantCulture) + "%";

    public static string Plain(double v) => v.ToString("0.##", CultureInfo.InvariantCulture);
}
