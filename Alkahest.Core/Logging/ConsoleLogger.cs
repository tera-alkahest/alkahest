using System;
using System.Text;

namespace Alkahest.Core.Logging
{
    public sealed class ConsoleLogger : ILogger
    {
        public static string Name => "console";

        readonly bool _timestamp;

        readonly bool _level;

        readonly bool _source;

        readonly bool _colors;

        readonly ConsoleColor _errorColor;

        readonly ConsoleColor _warningColor;

        readonly ConsoleColor _basicColor;

        readonly ConsoleColor _infoColor;

        readonly ConsoleColor _debugColor;

        public ConsoleLogger(bool timestamp, bool level, bool source, bool colors,
            ConsoleColor errorColor, ConsoleColor warningColor, ConsoleColor basicColor,
            ConsoleColor infoColor, ConsoleColor debugColor)
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

            _timestamp = timestamp;
            _level = level;
            _source = source;
            _colors = colors;
            _errorColor = errorColor;
            _warningColor = warningColor;
            _basicColor = basicColor;
            _infoColor = infoColor;
            _debugColor = debugColor;
        }

        public void Log(LogLevel level, string timestamp, Type source, string category, string message)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (message == null)
                throw new ArgumentNullException(nameof(message));

            ConsoleColor color;
            string lvl;

            switch (level.CheckValidity(nameof(level)))
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

            var sb = new StringBuilder();

            if (_timestamp && timestamp != null)
                sb.AppendFormat("[{0}] ", timestamp);

            if (_level)
                sb.AppendFormat("[{0}] ", lvl);

            if (_source)
            {
                sb.AppendFormat("{0}", source.Name);

                if (category != null)
                    sb.AppendFormat(" ({0})", category);

                sb.Append(": ");
            }

            sb.Append(message);

            try
            {
                if (_colors)
                    Console.ForegroundColor = color;

                (level <= LogLevel.Warning ? Console.Error : Console.Out).WriteLine(sb.ToString());
            }
            finally
            {
                Console.ResetColor();
            }
        }
    }
}
