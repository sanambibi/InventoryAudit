using Microsoft.AspNetCore.Mvc;
using InventoryAudit.Models;
using InventoryAudit.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InventoryAudit.Pages;

public class AuditModel(AuditService audit) : PageModel
{
    [BindProperty] public string? Batch { get; set; }
    [BindProperty] public bool Commit { get; set; }

    public AuditReport? Report { get; private set; }

    public void OnGet()
    {
        Batch = AuditService.SampleBatch;
    }

    public void OnPost()
    {
        // Never throws: every bad line is caught inside AuditService and reported.
        Report = audit.Run(Batch, Commit);
    }
}
