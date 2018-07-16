using System;
using System.IO;
using System.Threading;
using NLog;

namespace PiE520Downloader
{
    public static class Validator
    {
        private static void ValidateConfigFile(Config config)
        {
            var logger = LogManager.GetCurrentClassLogger();
            if (config.PartUsedAsName != "md5" && config.PartUsedAsName != "id")
            {
                const string errorMsg = "PartUsedAsName in Config needs to be either \"md5\" or \"id\"!";
                logger.Error("Validation failed! Read log file to see what error has occured!");
                logger.Fatal(errorMsg);
                Thread.Sleep(1000);
                throw new InvalidDataException(errorMsg);
            }
        }

        private static bool ValidateTagFile(Config config)
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

                return true;
            }

            logger.Info("Tag file exists.");
            return false;
        }

        public static void ValidateFiles()
        {
            var logger = LogManager.GetCurrentClassLogger();
            var config = Util.GetConfigFile(Util.Config);
            ValidateConfigFile(config);
            bool failed = ValidateTagFile(config);
            
            if (!Directory.Exists(config.DownloadDirectory))
            {
                Directory.CreateDirectory(config.DownloadDirectory);
            }

            if (failed)
            {
                logger.Error("Validation failed.");
                logger.Error("Please restart and try again.");
                Util.PauseConsole();
                Environment.Exit(-1); // Don't bother running the rest of the application without proper setup.
            }
            
            logger.Info("Validation complete.");
        }
    }
}