using System;
using System.IO;
using System.Text;

namespace Alkahest.Core.Logging
{
    public sealed class FileLogger : IDisposable, ILogger
    {
        public static readonly string Name = "file";

        readonly bool _timestamp;

        readonly bool _level;

        readonly bool _source;

        readonly StreamWriter _writer;

        bool _disposed;

        public FileLogger(bool timestamp, bool level, bool source, string directory,
            string fileNameFormat)
        {
            if (fileNameFormat == null)
                throw new ArgumentNullException(nameof(fileNameFormat));

            Directory.CreateDirectory(directory);

            _timestamp = timestamp;
            _level = level;
            _source = source;
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

            _writer.WriteLine(sb.ToString());
            _writer.Flush();
        }
    }
}
