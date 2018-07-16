using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;
using PiE520Downloader.E621Api;
using PiE520Downloader.Properties;

namespace PiE520Downloader
{
    internal static class Program
    {
        private static void EnumeratePostsFromTags(IEnumerable<string> tags, Config config, List<Post> posts, ILogger logger)
        {
            foreach (string tag in tags)
            {
                var newPosts = E621PostManager.GetPosts(tag).ToList();
                var existingPosts = new List<Post>();
                var files = Directory.EnumerateFileSystemEntries(config.DownloadDirectory).Select(Path.GetFileName).ToList();
                foreach (var post in newPosts)
                {
                    string fileName = $"{(config.PartUsedAsName == "id" ? post.Id.ToString() : post.Md5)}.{post.FileExt}";
                    foreach (string file in files)
                    {
                        if (file == fileName)
                        {
                            existingPosts.Add(post);
                        }
                    }
                }

                foreach (var post in existingPosts)
                {
                    newPosts.Remove(post);
                }

                posts.AddRange(newPosts);
                logger.Info(
                    $"{tag} Posts: {posts.Count} Total (+{newPosts.Count} New, {existingPosts.Count} Exists)");
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
            EnumeratePostsFromTags(tags, config, posts, logger);

            if (posts.Count > 0)
            {
                Downloader.MultiDownload(posts, logger);
                logger.Debug("Updating config...");
                config.LastRun = $"{DateTime.Today:yyyy/MM/dd}";
                Util.SaveConfig(config);
                logger.Debug("Config file updated.");
            }
            
            Util.PauseConsole();
        }
    }
}