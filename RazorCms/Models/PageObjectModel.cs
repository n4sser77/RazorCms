

using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace RazorCms.Models
{
    public class Page
    {


        public int? Id { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; }
        public string Slug { get; set; }
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
