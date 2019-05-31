using System;
using System.IO;

namespace Alkahest.Core.Logging.Loggers
{
    public sealed class FileLogger : IDisposable, ILogger
    {
        public const string Name = "file";

        readonly StreamWriter _writer;

        bool _disposed;

        public FileLogger(string directory, string fileNameFormat)
        {
            if (fileNameFormat == null)
                throw new ArgumentNullException(nameof(fileNameFormat));

            Directory.CreateDirectory(directory);

            _writer = new StreamWriter(File.Open(
                Path.Combine(directory, DateTime.Now.ToString(fileNameFormat) + ".log"),
                FileMode.Create, FileAccess.Write));
        }

        ~FileLogger()
        {
            RealDispose(false);
        }

        public void Dispose()
        {
            RealDispose(true);
            GC.SuppressFinalize(this);
        }

        void RealDispose(bool disposing)
        {
            _disposed = true;

            if (disposing)
                _writer.Dispose();
        }

        public void Log(LogLevel level, string timestamp, Type source, string category,
            string message)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (message == null)
                throw new ArgumentNullException(nameof(message));

            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            string lvl;

            switch (level.CheckValidity(nameof(level)))
            {
                case LogLevel.Error:
                    lvl = "E";
                    break;
                case LogLevel.Warning:
                    lvl = "W";
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
            _writer.Flush();
        }
    }
}
