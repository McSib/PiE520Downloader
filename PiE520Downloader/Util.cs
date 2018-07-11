using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NDesk.Options;
using Newtonsoft.Json;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace PiE520Downloader
{
    public static class Util
    {
        public const string Config = "config.txt";

        public static void ValidateFiles()
        {
            var logger = LogManager.GetCurrentClassLogger();
            logger.Info("Checking cache file...");

            var config = GetConfigFile(Config);
            bool failed = false;
            if (!File.Exists(config.CacheName))
            {
                logger.Error("Cache file doesn't exist!");
                File.WriteAllText(config.CacheName, "[]");
                logger.Error("Cache file created.");

                failed = true;
            }
            else
            {
                logger.Info("Cache file exists.");
            }

            logger.Info("Checking tag file...");
            if (!File.Exists(config.TagFile))
            {
                logger.Error("Tag file doesn't exist!");
                
                var writer = new StreamWriter(File.Create(config.TagFile));
                writer.Write("# Any line that starts with a pound sign is a comment and will be ignored by the program.\r\n" +
                             "# Much like e621dl, this uses that same syntax, so feel free to list as many artist/pools/groups as you like.\r\n");
                writer.Close();
                
                logger.Error("Tag file created.");
                
                failed = true;
            }
            else
            {
                logger.Info("Cache tag exists.");
            }

            if (failed)
            {
                logger.Error("Validation failed.");
                logger.Error("Please restart and try again.");
                Environment.Exit(-1);
            }
            
            logger.Info("Validation complete.");
        }
        
        public static void CreateLogger(IEnumerable<string> args)
        {
            int verbosity = DetermineLogLevel(args);
            var logConfig = GetLoggingConfig(verbosity);
            LogManager.Configuration = logConfig;
        }
        
        public static int DetermineLogLevel(IEnumerable<string> args)
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

        public static LoggingConfiguration GetLoggingConfig(int verbosity)
        {
            var config = new LoggingConfiguration();
            var logfile = new FileTarget("PiE520dl LogFile") {FileName = "log.txt"};
            var logconsole = new ConsoleTarget("PiE520dl Console");
            
            config.AddRule(verbosity == 1 ? LogLevel.Debug : LogLevel.Info, verbosity == 1 ? LogLevel.Error : LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
            return config;
        }
        
        public static Config GetConfigFile(string path)
        {
            if (File.Exists(path)) return JsonConvert.DeserializeObject<Config>(File.ReadAllText(Config));

            var logger = LogManager.GetCurrentClassLogger();
            logger.Error("Config file doesn't exist!");

            var config = new Config
            {
                CacheName = ".cache",
                CacheSize = 65536,
                CreateDirectories = false,
                DownloadDirectory = "downloades/",
                LastRun = DateTime.Today.ToShortDateString(),
                ParallelDownloads = 8,
                PartUsedAsName = "md5",
                TagFile = "tags.txt"
            };

            string json = JsonConvert.SerializeObject(config);
            var writer = new StreamWriter(File.Create(Config));
            writer.Write(json);
            writer.Close();
            
            logger.Error("Config file created.");
            logger.Error("Config file needs validation...");
            logger.Error("Starting validation check.");

            ValidateFiles();

            return config;

        }

        public static IEnumerable<string> ValidateTagFile()
        {
            var config = GetConfigFile(Config);
            var tags = File.ReadLines(config.TagFile).Where(i => !i.StartsWith("#"));
            var logger = LogManager.GetCurrentClassLogger();
            
            var validateTagFile = tags.ToList();
            if (!validateTagFile.Any())
            {
                logger.Error("No tags in file!");
                logger.Error("Please fill tag file.");
            }
            
            // Don't want to waste request on checking if tags are actually correct.
            logger.Info("Tag file validated.");

            return validateTagFile;
        }
    }
}