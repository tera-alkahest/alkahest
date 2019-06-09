using Alkahest.Core.Logging;
using Alkahest.Core.Reflection;
using Mono.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime;
using Theraot.Collections;

namespace Alkahest
{
    abstract class AlkahestCommand : Command
    {
        public string Title { get; }

        public virtual GCLatencyMode LatencyMode => GCLatencyMode.Batch;

        public AlkahestCommand(string title, string name, string help)
            : base(name, help)
        {
            Title = title;
            Run = args => throw new NotSupportedException();
        }

        protected abstract int Invoke(string[] args);

        public sealed override int Invoke(IEnumerable<string> arguments)
        {
            Log.Level = Configuration.LogLevel;
            Log.TimestampFormat = Configuration.LogTimestampFormat;
            Log.DiscardSources.AddRange(Configuration.DiscardLogSources);

            if (Configuration.Loggers.Contains(ConsoleLogger.Name))
                Log.Loggers.Add(new ConsoleLogger(Configuration.ColorsEnabled, Configuration.ErrorColor,
                    Configuration.WarningColor, Configuration.BasicColor, Configuration.InfoColor,
                    Configuration.DebugColor));

            var title = Console.Title;
            var mode = GCSettings.LatencyMode;

            try
            {
                var ver = Assembly.GetExecutingAssembly().GetInformationalVersion();

                Console.Title = $"{nameof(Alkahest)} {ver} - {Title}";
                GCSettings.LatencyMode = LatencyMode;

                return Invoke((Options?.Parse(arguments) ?? arguments).ToArray());
            }
            finally
            {
                GCSettings.LatencyMode = mode;
                Console.Title = title;
            }
        }
    }
}
