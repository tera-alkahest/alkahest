using System;

namespace Alkahest.Core.Logging
{
    public interface ILogger
    {
        void Log(LogLevel level, string timestamp, Type source, string category, string message);
    }
}
