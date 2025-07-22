using LeSi.Admin.Contracts.Logging;
using NLog;

namespace LeSi.Admin.Infrastructure.Logging
{
    public class NLogLogger : IAppLogger
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public void Trace(string message) => Logger.Trace(message);
        public void Debug(string message) => Logger.Debug(message);
        public void Info(string message) => Logger.Info(message);
        public void Warn(string message) => Logger.Warn(message);
        public void Error(string message, Exception ex = null) => Logger.Error(ex, message);
        public void Fatal(string message, Exception ex = null) => Logger.Fatal(ex, message);
    }
}