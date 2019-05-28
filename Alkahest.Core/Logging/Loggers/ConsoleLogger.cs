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

        public ConsoleLogger(bool colors, ConsoleColor errorColor, ConsoleColor warningColor,
            ConsoleColor basicColor, ConsoleColor infoColor, ConsoleColor debugColor)
        {
            static void CheckColor(ConsoleColor color, string name)
            {
                if (!Enum.IsDefined(typeof(ConsoleColor), color))
                    throw new ArgumentException("Invalid console color.", name);
            }

            CheckColor(errorColor, nameof(errorColor));
            CheckColor(warningColor, nameof(warningColor));
            CheckColor(basicColor, nameof(basicColor));
            CheckColor(infoColor, nameof(infoColor));
            CheckColor(debugColor, nameof(debugColor));

            _colors = colors;
            _errorColor = errorColor;
            _warningColor = warningColor;
            _basicColor = basicColor;
            _infoColor = infoColor;
            _debugColor = debugColor;
        }

        public void Log(LogLevel level, string timestamp, Type source, string category, string message)
        {
            level.CheckValidity(nameof(level));

            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (message == null)
                throw new ArgumentNullException(nameof(message));

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

            var console = level <= LogLevel.Warning ? Console.Error : Console.Out;

            if (_colors)
                Console.ForegroundColor = color;

            try
            {
                console.WriteLine($"{timestamp}[{lvl}] {source.Name}{category}: {message}");
            }
            finally
            {
                Console.ResetColor();
            }
        }
    }
}
