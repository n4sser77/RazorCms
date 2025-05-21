using System.Text.Json.Serialization;

namespace RazorCms.DTOs;
public class Block
{
    [JsonPropertyName("type")]
    public string Type { get; set; }
    [JsonPropertyName("order")]

    public int Order { get; set; }
    [JsonPropertyName("text")]
    public string? Text { get; set; }
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

