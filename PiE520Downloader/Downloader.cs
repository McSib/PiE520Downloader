using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using NLog;
using PiE520Downloader.E621Api;

namespace PiE520Downloader
{
    public static class Downloader
    {
        private static async Task SingleDownload(int i, IReadOnlyList<Post> posts, ProgressBar progressBar)
        {
            var logger = LogManager.GetCurrentClassLogger();
            var post = posts[i];
            
            using (var webClient = new WebClient())
            {
                webClient.Headers.Add("User-Agent", "PiE520Downloader");
                await webClient.DownloadFileTaskAsync(new Uri(post.file_url), $"downloads/{post.md5}.{post.file_ext}");
                progressBar.AddProgress();
            }
            
            logger.Debug($"Downloaded {post.file_url}");
        }

        public static void MultiDownload(List<Post> posts, ILogger logger)
        {
            logger.Debug("Downloading images with parallel for loop...");

            var config = Util.GetConfigFile(Util.Config);
            var progressBar = new ProgressBar();
            progressBar.SetLength(posts.Count);
            
            Parallel.For(0,
                posts.Count,
                new ParallelOptions() {MaxDegreeOfParallelism = 8},
                i =>
                {
                    SingleDownload(i, posts, progressBar).Wait();
                });
            
            logger.Debug("Successfully downloaded images.");
        }
    }
}