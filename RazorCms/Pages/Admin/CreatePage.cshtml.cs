using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorCms.Data;
using System.Security.Claims;

namespace RazorCms.Pages.Admin
{
    [Authorize]
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
        public string UserId { get; set; }
        public async Task<IActionResult> OnPostAsync()
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToPage("/Account/login");
            }
            Page.Slug = Page.Title.ToLower().Replace(" ", "-");


            if (!ModelState.IsValid)
                return Page();

            await _dbContext.Pages.AddAsync(Page);
            _dbContext.SaveChanges();
            return RedirectToPage("/Admin/Index");
        }
    }
}
