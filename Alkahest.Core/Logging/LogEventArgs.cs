using System;

namespace Alkahest.Core.Logging
{
    public sealed class LogEventArgs : EventArgs
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
