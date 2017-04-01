using System;

namespace Alkahest.Core.Logging.Loggers
{
    public sealed class ConsoleLogger : ILogger
    {
        public const string Name = "console";

        readonly bool _colors;

        readonly ConsoleColor _errorColor;

        readonly ConsoleColor _warningColor;

        readonly ConsoleColor _basicColor;

        readonly ConsoleColor _infoColor;

        readonly ConsoleColor _debugColor;

        public ConsoleLogger(bool colors, ConsoleColor errorColor,
            ConsoleColor warningColor, ConsoleColor basicColor,
            ConsoleColor infoColor, ConsoleColor debugColor)
        {
            _colors = colors;
            _errorColor = errorColor;
            _warningColor = warningColor;
            _basicColor = basicColor;
            _infoColor = infoColor;
            _debugColor = debugColor;
        }

        public void Log(LogLevel level, string timestamp, Type source,
            string category, string message)
        {
            ConsoleColor color;
            string lvl;

            switch (level)
            {
                case LogLevel.Error:
                    color = _errorColor;
                    lvl = "E";
                    break;
                case LogLevel.Warning:
                    color = _warningColor;
                    lvl = "W";
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

            timestamp = timestamp != null ? $"[{timestamp}] " : string.Empty;
            category = category != null ? $" ({category})" : string.Empty;

            var console = level <= LogLevel.Warning ?
                Console.Error : Console.Out;

            Console.ForegroundColor = color;
            console.WriteLine($"{timestamp}[{lvl}] {source.Name}{category}: {message}");
            Console.ResetColor();
        }
    }
}
