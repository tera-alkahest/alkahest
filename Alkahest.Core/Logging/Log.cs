using System;
using System.Collections.Generic;

namespace Alkahest.Core.Logging
{
    public sealed class Log
    {
        static LogLevel _level;

        public static LogLevel Level
        {
            get => _level;
            set => _level = value.CheckValidity(nameof(value));
        }

        public static string TimestampFormat { get; set; }

        public static ICollection<string> DiscardSources { get; } = new HashSet<string>();

        public static ICollection<ILogger> Loggers { get; } = new HashSet<ILogger>();

        static readonly object _lock = new object();

        public static event EventHandler<LogEventArgs> MessageLogged;

        public Type Source { get; }

        public string Category { get; }

        public Log(Type source, string category = null)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Category = category;
        }

        public void Error(string format, params object[] args)
        {
            LogMessage(LogLevel.Error, format, args);
        }

        public void Warning(string format, params object[] args)
        {
            LogMessage(LogLevel.Warning, format, args);
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

        bool ShouldLog(LogLevel level)
        {
            return level <= LogLevel.Warning || (level <= _level && !DiscardSources.Contains(Source.Name));
        }

        void LogMessage(LogLevel level, string format, params object[] args)
        {
            if (format == null)
                throw new ArgumentNullException(nameof(format));

            if (args == null)
                throw new ArgumentNullException(nameof(args));

            if (!ShouldLog(level))
                return;

            var msg = string.Format(format, args);
            var now = DateTime.Now;
            var stamp = TimestampFormat;

            stamp = !string.IsNullOrWhiteSpace(stamp) ? now.ToString(stamp) : null;

            lock (_lock)
            {
                MessageLogged?.Invoke(this, new LogEventArgs(level, now, msg));

                foreach (var logger in Loggers)
                    logger?.Log(level, stamp, Source, Category, msg);
            }
        }
    }
}
