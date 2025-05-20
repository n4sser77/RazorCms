

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
        [JsonPropertyName("isVisible")]
        public bool IsVisible { get; set; }
        [JsonPropertyName("Order")]
        public int OrderIndex { get; set; }

    }
}
