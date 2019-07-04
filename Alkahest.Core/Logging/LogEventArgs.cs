using System;

namespace Alkahest.Core.Logging
{
    public readonly struct LogEventArgs
    {
        public LogLevel Level { get; }

        public DateTime Timestamp { get; }

        public string Message { get; }

        internal LogEventArgs(LogLevel level, DateTime timestamp, string message)
        {
            Level = level;
            Timestamp = timestamp;
            Message = message;
        }
    }
}
