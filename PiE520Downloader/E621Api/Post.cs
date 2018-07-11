using System.Collections.Generic;
using Newtonsoft.Json;

namespace PiE520Downloader.E621Api
{
    public class CreatedTime
    {
        [JsonProperty("json_class")] public string JsonClass { get; set; }
        [JsonProperty("s")] public ulong Seconds { get; set; }
        [JsonProperty("n")] public ulong Nanos { get; set; }
    }

    public class Post
    {
        [JsonProperty("id")] public ulong Id { get; set; }
        [JsonProperty("tags")] public string Tags { get; set; }
        [JsonProperty("locked_tags")] public string LockedTags { get; set; }
        [JsonProperty("descrition")] public string Description { get; set; }
        [JsonProperty("created_at")] public CreatedTime CreatedAt { get; set; }
        [JsonProperty("creator_id")] public ulong? CreatorId { get; set; }
        [JsonProperty("author")] public string Author { get; set; }
        [JsonProperty("change")] public ulong Change { get; set; }
        [JsonProperty("source")] public string Source { get; set; }
        [JsonProperty("score")] public int Score { get; set; }
        [JsonProperty("fav_count")] public ulong FavCount { get; set; }
        [JsonProperty("md5")] public string Md5 { get; set; }
        [JsonProperty("file_size")] public ulong FileSize { get; set; }
        [JsonProperty("file_url")] public string FileUrl { get; set; }
        [JsonProperty("file_ext")] public string FileExt { get; set; }
        [JsonProperty("preview_size")] public ulong PreviewSize { get; set; }
        [JsonProperty("preview_url")] public string PreviewUrl { get; set; }
        [JsonProperty("preview_ext")] public string PreviewExt { get; set; }
        [JsonProperty("sample_size")] public ulong SampleSize { get; set; }
        [JsonProperty("sample_url")] public string SampleUrl { get; set; }
        [JsonProperty("sample_ext")] public string SampleExt { get; set; }
        [JsonProperty("rating")] public string Rating { get; set; }
        [JsonProperty("status")] public string Status { get; set; }
        [JsonProperty("width")] public uint Width { get; set; }
        [JsonProperty("height")] public uint Height { get; set; }
        [JsonProperty("has_comments")] public bool HasComments { get; set; }
        [JsonProperty("has_notes")] public bool HasNotes { get; set; }
        [JsonProperty("has_children")] public bool HasChildren { get; set; }
        [JsonProperty("children")] public string Children { get; set; }
        [JsonProperty("parent_id")] public ulong? ParentId { get; set; }
        [JsonProperty("artist")] public IList<string> Artist { get; set; }
        [JsonProperty("sources")] public IList<string> Sources { get; set; }
    }
}