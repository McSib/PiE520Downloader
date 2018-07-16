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

        public static void PauseConsole()
        {
            Console.WriteLine();
            Console.Write("Press any key to continue...");
            Console.ReadKey(true);
        }
        
        public static IEnumerable<string> GetTags()
        {
            var config = GetConfigFile(Config);
            var tags = File.ReadLines(config.TagFile).Where(i => !i.StartsWith("#"));
            var logger = LogManager.GetCurrentClassLogger();
            
            var tagList = tags.ToList();
            if (!tagList.Any())
            {
                logger.Error("No tags in file!");
                logger.Error("Please fill tag file.");
            }

            return tagList;
        }

        public static void CreateLogger(IEnumerable<string> args)
        {
            int verbosity = DetermineLogLevel(args);
            var logConfig = GetLoggingConfig(verbosity);
            LogManager.Configuration = logConfig;
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

        private static LoggingConfiguration GetLoggingConfig(int verbosity)
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
            while (true)
            {
                if (File.Exists(path)) return JsonConvert.DeserializeObject<Config>(File.ReadAllText(Config));

                var logger = LogManager.GetCurrentClassLogger();
                logger.Error("Config file doesn't exist!");

                CreateConfig();
                Validator.ValidateFiles();
            }
        }

        public static void SaveConfig(Config config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            string json = JsonConvert.SerializeObject(config);
            File.WriteAllText(Config, json);
        }

        private static void CreateConfig()
        {
            var logger = LogManager.GetCurrentClassLogger();
            var newConfig = new Config
            {
                CreateDirectories = false,
                DownloadDirectory = "downloads/",
                LastRun = $"{DateTime.Today:yyyy/MM/dd}",
                PartUsedAsName = "md5",
                TagFile = "tags.txt"
            };

            string json = JsonConvert.SerializeObject(newConfig);
            var writer = new StreamWriter(File.Create(Config));
            writer.Write(json);
            writer.Close();

            logger.Error("Config file created.");
            logger.Error("Config file needs validation...");
            logger.Error("Starting validation check.");
        }
    }
}