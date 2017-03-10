using System;

namespace Alkahest.Core.Logging.Loggers
{
    public sealed class ConsoleLogger : ILogger
    {
        public const string Name = "console";

        readonly bool _colors;

        readonly ConsoleColor _errorColor;

        readonly ConsoleColor _basicColor;

        readonly ConsoleColor _infoColor;

        readonly ConsoleColor _debugColor;

        public ConsoleLogger(bool colors, ConsoleColor errorColor,
            ConsoleColor basicColor, ConsoleColor infoColor,
            ConsoleColor debugColor)
        {
            _colors = colors;
            _errorColor = errorColor;
            _basicColor = basicColor;
            _infoColor = infoColor;
            _debugColor = debugColor;
        }

        public void Log(LogLevel level, string timestamp,
            string source, string message)
        {
            ConsoleColor color;
            string lvl;

            switch (level)
            {
                case LogLevel.Error:
                    color = _errorColor;
                    lvl = "E";
                    break;
                case LogLevel.Basic:
                    color = _basicColor;
                    lvl = "B";
                    break;
                case LogLevel.Info:
                    color = _infoColor;
                    lvl = "I";
                    break;
                case LogLevel.Debug:
                    color = _debugColor;
                    lvl = "D";
                    break;
                default:
                    throw Assert.Unreachable();
            }

            timestamp = timestamp != string.Empty ? $"[{timestamp}] " : string.Empty;

            Console.ForegroundColor = color;
            Console.WriteLine($"{timestamp}[{lvl}] {source}: {message}");
            Console.ResetColor();
        }
    }
}
