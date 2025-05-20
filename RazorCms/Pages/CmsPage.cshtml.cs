using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorCms.Data;
using RazorCms.DTOs;
using System.Text.Json;

namespace RazorCms.Pages
{
    public class CmsPageModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;
        public CmsPageModel(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Models.Page Page { get; set; }
        public List<Block> Blocks { get; set; }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (id == 0)
            {
                return RedirectToPage("/Index");
            }

            var page = await _dbContext.Pages.FindAsync(id);
            if (page == null)
            {
                return RedirectToPage("/Index");
            }


            this.Page = page;
            var blocks = JsonSerializer.Deserialize<List<Block>>(page.Content);
            if (blocks == null)
            {
                Blocks = new List<Block>();
                return Page();
            }

            Blocks = blocks;
            return Page();

        }
    }
}
