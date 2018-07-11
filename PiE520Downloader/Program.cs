using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NDesk.Options;
using Newtonsoft.Json;
using NLog;
using NLog.Config;
using NLog.Targets;
using PiE520Downloader.E621Api;

namespace PiE520Downloader
{
    internal static class Program
    {
        private const string Version = "1.0.0";
        
        private static LoggingConfiguration GetConfig(int verbosity)
        {
            var config = new LoggingConfiguration();
            var logfile = new FileTarget("PiE520dl LogFile") {FileName = "log.txt"};
            var logconsole = new ConsoleTarget("PiE520dl Console");
            
            config.AddRule(verbosity == 1 ? LogLevel.Debug : LogLevel.Info, verbosity == 1 ? LogLevel.Error : LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
            return config;
        }

        private static int DetermineLogLevel(IEnumerable<string> args)
        {
            int verbosity = 0;
            var p = new OptionSet()
            {
                {"v", "Prints debug statements.", v => verbosity++}
            };

            try
            {
                p.Parse(args);
            }
            catch (OptionException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }

            return verbosity;
        }

        private static Logger GetLogger(IEnumerable<string> args)
        {
            int verbosity = DetermineLogLevel(args);
            var logConfig = GetConfig(verbosity);
            LogManager.Configuration = logConfig;

            var logger = LogManager.GetCurrentClassLogger();
            return logger;
        }

        public static void Main(string[] args)
        {
            var logger = GetLogger(args);
            logger.Debug("Logger intialized.");
            logger.Info($"Running PiE520Downloader version {Version}");
            
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
