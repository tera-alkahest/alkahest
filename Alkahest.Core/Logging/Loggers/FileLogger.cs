using System;
using System.IO;

namespace Alkahest.Core.Logging.Loggers
{
    public sealed class FileLogger : ILogger
    {
        public const string Name = "file";

        readonly StreamWriter _writer;

        public FileLogger(string directory, string fileNameFormat)
        {
            Directory.CreateDirectory(directory);

            _writer = new StreamWriter(File.OpenWrite(Path.Combine(directory,
                DateTime.Now.ToString(fileNameFormat) + ".log")));
        }

        public void Log(LogLevel level, string timestamp, Type source,
            string category, string message)
        {
            string lvl;

            switch (level)
            {
                case LogLevel.Error:
                    lvl = "E";
                    break;
                case LogLevel.Basic:
                    lvl = "B";
                    break;
                case LogLevel.Info:
                    lvl = "I";
                    break;
                case LogLevel.Debug:
                    lvl = "D";
                    break;
                default:
                    throw Assert.Unreachable();
            }

            timestamp = timestamp != null ? $"[{timestamp}] " : string.Empty;
            category = category != null ? $" ({category})" : string.Empty;

            _writer.WriteLine($"{timestamp}[{lvl}] {source.Name}{category}: {message}");
        }
    }
}
