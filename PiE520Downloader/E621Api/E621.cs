using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using NLog;

namespace PiE520Downloader.E621Api
{
    public static class E621
    {
        private const int MaxLimit = 100;

        public static IEnumerable<Post> GetPosts(string tags)
        {
            var config = Util.GetConfigFile(Util.Config);
            string formatedTags = tags.Replace(" ", "+");
            var datetime = DateTime.Parse(config.LastRun);
            string date = $"date:>={datetime:yyyy/MM/dd}";
            int currentPage = 1;

            var logger = LogManager.GetCurrentClassLogger();
            var posts = new List<Post>();
            bool elementsExist = true;
            do
            {
                using (var webClient = new WebClient())
                {
                    string uri = $"http://e621.net/post/index.json?tags={formatedTags}+{date}&page={currentPage}&limit={MaxLimit}";
                    logger.Debug($"Url: {uri}");
                    logger.Debug($"Grabbing page: {currentPage}");
                
                    webClient.Headers.Add("User-Agent", "PiE520Downloader");
                    var newPosts = JsonConvert.DeserializeObject<List<Post>>(webClient.DownloadString(uri));
                    if (newPosts.Count > 0)
                    {
                        posts.AddRange(newPosts);
                        logger.Debug($"{posts.Count} total posts grabbed.");
                        currentPage++;
                    }
                    else
                    {
                        logger.Debug($"All posts with tags \"{tags}\" grabbed from {DateTime.Parse(config.LastRun):yyyy/MM/dd} to {DateTime.Now:yyyy/MM/dd}");
                        elementsExist = false;
                    }
                }
                
                // This is to prevent a DOS from the server.
//                Thread.Sleep(500);
            } while (elementsExist);

            return posts;
        }
    }
}