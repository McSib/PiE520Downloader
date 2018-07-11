using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NLog;
using PiE520Downloader.E621Api;
using PiE520Downloader.Properties;

namespace PiE520Downloader
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var logger = LogManager.GetCurrentClassLogger();
            logger.Debug("Logger intialized.");
            logger.Info($"Running PiE520Downloader version {ProgramVersion.Version}");
            
            var config = Util.GetConfigFile(Util.Config);
            Util.ValidateFiles();
            
            var posts = new List<Post>();
            var tags = Util.ValidateTagFile().ToList();
            var cache = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(config.CacheName));
            foreach (string tag in tags)
            {
                var newPosts = E621PostManager.GetPosts(tag).ToList();

                var cachedPosts = (from post in newPosts from md5Checks in cache where post.md5 == md5Checks select post).ToList();
                foreach (var cachedPost in cachedPosts)
                {
                    newPosts.Remove(cachedPost);
                }

                posts.AddRange(newPosts);
                logger.Info($"{tag} Posts: {posts.Count} Total ({newPosts.Count} New, {cachedPosts.Count} Exists, {cachedPosts.Count} Cached)");
            }

            Downloader.MultiDownload(posts, logger);
            
            cache.AddRange(posts.Select(i => i.md5));
            string jsonCache = JsonConvert.SerializeObject(cache);
            File.WriteAllText(".cache", jsonCache);
        }
    }
}
