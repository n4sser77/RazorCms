using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorCms.Data;

namespace RazorCms.Pages.Admin
{
    public class CreatePageModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;

        public CreatePageModel(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [BindProperty]
        public Models.Page Page { get; set; } = new Models.Page();
        //public async Task<IActionResult> OnGetAsync()
        //{

        //}

        public async Task<IActionResult> OnPostAsync()
        {
            Page.Slug = Page.Title.ToLower().Replace(" ", "-");


            if (!ModelState.IsValid)
                return Page();

            await _dbContext.Pages.AddAsync(Page);
            _dbContext.SaveChanges();
            return RedirectToPage("/Admin/Index");
        }
    }
}
