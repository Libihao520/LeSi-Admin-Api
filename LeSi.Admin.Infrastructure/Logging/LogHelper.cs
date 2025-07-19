using NLog;

namespace LeSi.Admin.Infrastructure.Logging
{
    public static class LogHelper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static void Trace(string message)
        {
            Logger.Trace(message);
        }

        public static void Debug(string message)
        {
            Logger.Debug(message);
        }

        public static void Info(string message)
        {
            Logger.Info(message);
        }

        public static void Warn(string message)
        {
            Logger.Warn(message);
        }

        public static void Error(string message, System.Exception ex = null)
        {
            if (ex != null)
            {
                Logger.Error(ex, message);
            }
            else
            {
                Logger.Error(message);
            }
        }

        public static void Fatal(string message, System.Exception ex = null)
        {
            if (ex != null)
            {
                Logger.Fatal(ex, message);
            }
            else
            {
                Logger.Fatal(message);
            }
        }
    }
}