using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorCms.Data;

namespace RazorCms.Pages.Admin;
[Authorize]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;
    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public int PageId { get; set; }

    [TempData]
    public string StatusMessage { get; set; }



    public Dictionary<string, int> TotalVisitors { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        TotalVisitors = new Dictionary<string, int>();
        foreach (var page in _context.Pages)
        {
            var totalVisits = _context.VisitLogs.Count(v => v.PageVisited == $"/{page.Id}");
            TotalVisitors.Add($"/{page.Id}", totalVisits);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (PageId == 0)
        {
            StatusMessage = "Error: Page not found.";

            return RedirectToPage();
        }

        var pageToDelete = await _context.Pages.FindAsync(PageId);

        if (pageToDelete is null)
        {
            StatusMessage = "Error: Page not found.";

            return RedirectToPage();

        }

        _context.Pages.Remove(pageToDelete);
        await _context.SaveChangesAsync();
        StatusMessage = $"Page '{pageToDelete.Title}' deleted successfully!";

        return RedirectToPage();
    }
}
