using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace RazorCms.DTOs;
public class BatchUpdateDto
{
    [JsonPropertyName("pageId")]
    public int PageId { get; set; }

    [JsonPropertyName("page")]
    public PageDto Page { get; set; }

    [JsonPropertyName("editedBlocks")]
    public List<Block> EditedBlocks { get; set; }

    [JsonPropertyName("deletedBlockIds")]
    public List<string> DeletedBlockIds { get; set; }

    [JsonPropertyName("addedBlocks")]
    public List<Block> AddedBlocks { get; set; }

}