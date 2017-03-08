using System;
using System.Collections.Generic;

namespace Alkahest.Core.Logging
{
    public sealed class Log
    {
        public static LogLevel Level { get; set; }

        public static string TimestampFormat { get; set; }

        public static List<string> DiscardSources { get; } = new List<string>();

        public static List<ILogger> Loggers { get; } = new List<ILogger>();

        static readonly object _lock = new object();

        readonly string _source;

        public Log(Type source)
        {
            _source = source.Name;
        }

        public void Error(string format, params object[] args)
        {
            LogMessage(LogLevel.Error, _source, format, args);
        }

        public void Basic(string format, params object[] args)
        {
            LogMessage(LogLevel.Basic, _source, format, args);
        }

        public void Info(string format, params object[] args)
        {
            LogMessage(LogLevel.Info, _source, format, args);
        }

        public void Debug(string format, params object[] args)
        {
            LogMessage(LogLevel.Debug, _source, format, args);
        }

        static void LogMessage(LogLevel level, string source,
            string format, params object[] args)
        {
            if (level > Level || (level != LogLevel.Error && DiscardSources.Contains(source)))
                return;

            var msg = args.Length != 0 ? string.Format(format, args) : format;
            var stamp = TimestampFormat != string.Empty ?
                DateTime.Now.ToString(TimestampFormat) : string.Empty;

            lock (_lock)
                foreach (var logger in Loggers)
                    logger.Log(level, stamp, source, msg);
        }
    }
}
