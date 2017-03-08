namespace Alkahest.Core.Logging
{
    public interface ILogger
    {
        void Log(LogLevel level, string timestamp,
            string source, string message);
    }
}
