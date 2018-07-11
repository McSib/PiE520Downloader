using System;
using System.IO;
using NLog;

namespace PiE520Downloader
{
    public static class Validator
    {
        private static void ValidateCache(Config config)
        {
            var logger = LogManager.GetCurrentClassLogger();
            if (!File.Exists(config.CacheName))
            {
                logger.Error("Cache file doesn't exist!");
                File.WriteAllText(config.CacheName, "[]");
                logger.Error("Cache file created.");
            }
            else
            {
                logger.Info("Cache file exists.");
            }
        }

        private static void ValidateTagFile(Config config, ref bool failed)
        {
            var logger = LogManager.GetCurrentClassLogger();
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
                logger.Info("Tag file exists.");
            }
        }

        public static void ValidateFiles()
        {
            var logger = LogManager.GetCurrentClassLogger();
            logger.Info("Checking cache file...");

            var config = Util.GetConfigFile(Util.Config);
            bool failed = false;
            ValidateCache(config);
            ValidateTagFile(config, ref failed);

            if (failed)
            {
                logger.Error("Validation failed.");
                logger.Error("Please restart and try again.");
                Environment.Exit(-1); // Don't bother running the rest of the application without proper setup.
            }
            
            logger.Info("Validation complete.");
        }
    }
}