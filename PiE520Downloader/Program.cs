using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using Newtonsoft.Json;
using NLog;
using PiE520Downloader.E621Api;
using PiE520Downloader.Properties;

namespace PiE520Downloader
{
    internal static class Program
    {
        private static void EnumeratePostsFromTags(IEnumerable<string> tags, Config config, IReadOnlyCollection<string> cache, List<Post> posts, ILogger logger)
        {
            foreach (string tag in tags)
            {
                var newPosts = E621PostManager.GetPosts(tag).ToList();
                var existingPost = new List<Post>();
                var files = Directory.EnumerateFileSystemEntries(config.DownloadDirectory).Select(Path.GetFileName).ToList();
                foreach (var post in newPosts)
                {
                    string fileName = $"{post.Md5}.{post.FileExt}";
                    existingPost.Add((from file in files where fileName == file select post).FirstOrDefault());
                }

                foreach (var post in existingPost)
                {
                    newPosts.Remove(post);
                }

                var cachedPosts =
                    (from post in newPosts from md5Checks in cache where post.Md5 == md5Checks select post).ToList();
                foreach (var cachedPost in cachedPosts)
                {
                    newPosts.Remove(cachedPost);
                }

                posts.AddRange(newPosts);
                logger.Info(
                    $"{tag} Posts: {posts.Count} Total ({newPosts.Count} New, {existingPost.Count} Exists, {cachedPosts.Count} Cached)");
            }
        }

        public static void Main(string[] args)
        {
            Util.CreateLogger(args);
            var logger = LogManager.GetCurrentClassLogger();
            logger.Debug("Logger intialized.");
            logger.Info($"Running PiE520Downloader version {ProgramVersion.Version}");

            var config = Util.GetConfigFile(Util.Config);
            Validator.ValidateFiles();

            var posts = new List<Post>();
            var tags = Util.GetTags().ToList();
            var cache = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(config.CacheName));
            EnumeratePostsFromTags(tags, config, cache, posts, logger);

            if (posts.Count > 0)
            {
                Downloader.MultiDownload(posts, logger);
                
                cache.AddRange(posts.Select(i => i.Md5));
                string jsonCache = JsonConvert.SerializeObject(cache);
                File.WriteAllText(".cache", jsonCache);
            }
        }
    }
}