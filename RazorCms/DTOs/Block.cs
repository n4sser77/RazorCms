using System.Text.Json.Serialization;

namespace RazorCms.DTOs;
public class Block
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }
    
    [JsonPropertyName("text")]
    public string? Text { get; set; }
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

