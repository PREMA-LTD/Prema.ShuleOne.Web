using System.Reflection;
using System.Runtime.InteropServices;
using Prema.ShuleOne.Web.Server.Telegram;
using Serilog;

namespace Prema.ShuleOne.Web.Server.Logging
{
    public class Logger
    {
        private static string DirectoryPath;
        private static string FilePath;

        private readonly TelegramBot telegramBot;

        public Logger(TelegramBot _telegramBot)
        {
            telegramBot = _telegramBot;
        }

        private static void CheckFolder()
        {
            string logDay = String.Empty;

            if (!OperatingSystem.IsWindows())
            {
                logDay = $"//.log";
            }
            else
            {
                logDay = $"\\.log";
            }

            DirectoryPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            string dirPath = Path.Combine(DirectoryPath, "Logs");
            FilePath = $"{dirPath}{logDay}";

            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
        }

        public void WriteToLog(string message, string messageType)
        {
            CheckFolder();

            var log = new LoggerConfiguration()
             .WriteTo.File(FilePath, rollingInterval: RollingInterval.Day)
             .CreateLogger();

            switch (messageType)
            {
                case "Information":
                    log.Information(message);
                    telegramBot.SendMessage(message);
                    break;
                case "Error":
                    log.Error(message);
                    telegramBot.SendMessage("⚠️ An error occured!");
                    break;
                case "Debug":
                    log.Debug(message);
                    break;
            }

        }
    }
    public static class OperatingSystem
    {
        public static bool IsWindows() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        public static bool IsMacOS() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        public static bool IsLinux() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    }
}