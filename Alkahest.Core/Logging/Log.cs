using System;
using System.Collections.Generic;

namespace Alkahest.Core.Logging
{
    public sealed class Log
    {
        public static LogLevel Level { get; set; }

        public static string TimestampFormat { get; set; }

        public static ICollection<string> DiscardSources { get; } =
            new HashSet<string>();

        public static ICollection<ILogger> Loggers { get; } =
            new HashSet<ILogger>();

        public static event EventHandler<LogEventArgs> MessageLogged;

        static readonly object _lock = new object();

        public Type Source { get; }

        public string Category { get; }

        public Log(Type source)
            : this(source, null)
        {
        }

        public Log(Type source, string category)
        {
            Source = source;
            Category = category;
        }

        public void Error(string format, params object[] args)
        {
            LogMessage(LogLevel.Error, format, args);
        }

        public void Basic(string format, params object[] args)
        {
            LogMessage(LogLevel.Basic, format, args);
        }

        public void Info(string format, params object[] args)
        {
            LogMessage(LogLevel.Info, format, args);
        }

        public void Debug(string format, params object[] args)
        {
            LogMessage(LogLevel.Debug, format, args);
        }

        void LogMessage(LogLevel level, string format, params object[] args)
        {
            if (level > Level || (level != LogLevel.Error &&
                DiscardSources.Contains(Source.Name)))
                return;

            var msg = args.Length != 0 ? string.Format(format, args) : format;
            var now = DateTime.Now;
            var stamp = TimestampFormat != string.Empty ?
                now.ToString(TimestampFormat) : null;

            lock (_lock)
            {
                MessageLogged?.Invoke(this, new LogEventArgs(level, now, msg));

                foreach (var logger in Loggers)
                    logger.Log(level, stamp, Source, Category, msg);
            }
        }
    }
}
