using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace PiE520Downloader.E621Api
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class CreatedTime
    {
        public string json_class { get; set; }
        public ulong s { get; set; }
        public ulong n { get; set; }
    }
    
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class Post
    {
        public ulong id { get; set; }
        public string tags { get; set; }
        public string locked_tags { get; set; }
        public string description { get; set; }
        public CreatedTime created_at { get; set; }
        public ulong? creator_id { get; set; }
        public string author { get; set; }
        public ulong change { get; set; }
        public string source { get; set; }
        public int score { get; set; }
        public ulong fav_count { get; set; }
        public string md5 { get; set; }
        public ulong file_size { get; set; }
        public string file_url { get; set; }
        public string file_ext { get; set; }
        public ulong preview_size { get; set; }
        public string preview_url { get; set; }
        public string preview_ext { get; set; }
        public ulong sample_size { get; set; }
        public string sample_url { get; set; }
        public string sample_ext { get; set; }
        public string rating { get; set; }
        public string status { get; set; }
        public uint width { get; set; }
        public uint height { get; set; }
        public bool has_comments { get; set; }
        public bool has_notes { get; set; }
        public bool has_children { get; set; }
        public string children { get; set; }
        public ulong? parent_id { get; set; }
        public IList<string> artist { get; set; }
        public IList<string> sources { get; set; }
    }
}