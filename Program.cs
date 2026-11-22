using InventoryAudit.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
// In-memory store (no database needed). Singleton so data lives while the app runs.
builder.Services.AddSingleton<InventoryService>();
builder.Services.AddSingleton<AuditService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();

app.Run();
