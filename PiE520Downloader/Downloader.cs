using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NLog;
using PiE520Downloader.E621Api;

namespace PiE520Downloader
{
    public static class Downloader
    {
        private static async Task SingleDownload(Post post, string subDirectory, string filename,
            ProgressBar progressBar)
        {
            var logger = LogManager.GetCurrentClassLogger();

            using (var webClient = new WebClient())
            {
                webClient.Headers.Add("User-Agent", "PiE520Downloader");
                await webClient.DownloadFileTaskAsync(new Uri(post.FileUrl),
                    $"downloads/{subDirectory}{filename}.{post.FileExt}");
                progressBar.AddProgress();
            }

            logger.Debug($"Downloaded {post.FileUrl}");
        }

        public static void MultiDownload(List<Post> posts, ILogger logger)
        {
            logger.Debug("Downloading images with parallel for loop...");

            var config = Util.GetConfigFile(Util.Config);
            var tags = Util.GetTags();
            var progressBar = new ProgressBar();
            progressBar.SetLength(posts.Count);

            if (!Directory.Exists(config.DownloadDirectory))
            {
                Directory.CreateDirectory(config.DownloadDirectory);
            }
            
            Parallel.For(0,
                posts.Count,
                new ParallelOptions() {MaxDegreeOfParallelism = 8},
                i =>
                {
                    var post = posts[i];
                    string filename = config.PartUsedAsName == "id" ? post.Id.ToString() : post.Md5;
                    string subDirectory = string.Empty;
                    if (config.CreateDirectories)
                    {
                        foreach (string tag in tags)
                        {
                            string subTag = tag.Split(' ').FirstOrDefault(j => !j.StartsWith("-"));
                            var subPostTags = post.Tags.Split(' ').Where(j => j != string.Empty).Select(j => j);
                            if (subPostTags.Any(postTag => subTag == postTag))
                            {
                                subDirectory = tag;
                                Directory.CreateDirectory($"{config.DownloadDirectory}{subDirectory}/");
                                break;
                            }
                        }
                    }

                    SingleDownload(post, subDirectory == string.Empty ? subDirectory : $"{subDirectory}/", filename,
                        progressBar).Wait();
                });

            logger.Debug("Successfully downloaded images.");
        }
    }
}