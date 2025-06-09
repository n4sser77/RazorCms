

using Microsoft.AspNetCore.Identity;
using RazorCms.Data;
using System.Text.Json.Serialization;

namespace RazorCms.Models
{
    public class Page
    {


        public int? Id { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("blocks")]
        public string Content { get; set; }

        [JsonPropertyName("isHidden")]
        public bool IsHidden { get; set; }

        [JsonPropertyName("orderIndex")]
        public int OrderIndex { get; set; }

        [JsonPropertyName("userId")]
        public string UserId { get; set; }


    }




}



namespace RazorCms.Helpers
{
    public static class PageHelper
    {
        public static async Task AssignNextOrderIndexAsync(Models.Page page, ApplicationDbContext context)
        {
            // Count current pages in DB
            int pageCount =  context.Pages.Count();

            // Set the next order index
            page.OrderIndex = pageCount;
        }
    }
}

