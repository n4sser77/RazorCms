using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorCms.Data;
using RazorCms.DTOs;
using System.Security.Claims;
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



        public string UserId { get; set; }
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
            List<Block> blocks = new List<Block>();
            bool needsUpdate = false;
            if (!string.IsNullOrEmpty(page.Content))
            {
                try
                {

                    blocks = JsonSerializer.Deserialize<List<Block>>(page.Content);
                    foreach (var block in blocks)
                    {
                        if (string.IsNullOrEmpty(block.Id))
                        {
                            block.Id = Guid.NewGuid().ToString();
                            needsUpdate = true;

                        }

                    }

                    if (needsUpdate)
                    {
                        page.Content = JsonSerializer.Serialize(blocks);
                        _dbContext.Pages.Update(page);
                        await _dbContext.SaveChangesAsync();
                    }
                }
                catch (Exception e)
                {


                }
            }

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
