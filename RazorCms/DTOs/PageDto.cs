﻿using System.Text.Json.Serialization;

namespace RazorCms.DTOs
{
    public class PageDto
    {

        public int? Id { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("blocks")]
        public List<Block> Blocks { get; set; }
        [JsonPropertyName("isHidden")]
        public bool IsHidden { get; set; }

        public string UserId { get; set; }

    }

}
