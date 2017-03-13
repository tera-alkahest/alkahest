using System;
using System.Collections.Generic;

namespace Alkahest.Core.Logging
{
    public sealed class Log
    {
        public static LogLevel Level { get; set; }

        public static string TimestampFormat { get; set; } = string.Empty;

        public static ICollection<string> DiscardSources { get; } = new HashSet<string>();

        public static ICollection<ILogger> Loggers { get; } = new HashSet<ILogger>();

        static readonly object _lock = new object();

        readonly Type _source;

        readonly string _category;

        public Log(Type source)
            : this(source, null)
        {
        }

        public Log(Type source, string category)
        {
            _source = source;
            _category = category;
        }

        public void Error(string format, params object[] args)
        {
            LogMessage(LogLevel.Error, _source, _category, format, args);
        }

        public void Basic(string format, params object[] args)
        {
            LogMessage(LogLevel.Basic, _source, _category, format, args);
        }

        public void Info(string format, params object[] args)
        {
            LogMessage(LogLevel.Info, _source, _category, format, args);
        }

        public void Debug(string format, params object[] args)
        {
            LogMessage(LogLevel.Debug, _source, _category, format, args);
        }

        static void LogMessage(LogLevel level, Type source, string category,
            string format, params object[] args)
        {
            if (level > Level || (level != LogLevel.Error &&
                DiscardSources.Contains(source.Name)))
                return;

            var msg = args.Length != 0 ? string.Format(format, args) : format;
            var stamp = TimestampFormat != string.Empty ?
                DateTime.Now.ToString(TimestampFormat) : string.Empty;

            lock (_lock)
                foreach (var logger in Loggers)
                    logger.Log(level, stamp, source, category, msg);
        }
    }
}
