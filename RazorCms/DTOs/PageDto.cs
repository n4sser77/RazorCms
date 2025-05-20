using System.Text.Json.Serialization;

namespace RazorCms.DTOs
{
    public class PageDto
    {

        public int? Id { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; }
        public string Slug { get; set; }
        [JsonPropertyName("blocks")]
        public List<Block> Blocks { get; set; }
        [JsonPropertyName("isVisible")]
        public bool IsVisible { get; set; }
        [JsonPropertyName("Order")]
        public int OrderIndex { get; set; }

    }

}
