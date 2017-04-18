using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Security;
using Mono.Options;
using Alkahest.Core;
using Alkahest.Core.Logging;
using Alkahest.Core.Logging.Loggers;
using Alkahest.Extractor.Commands;

namespace Alkahest.Extractor
{
    static class Application
    {
        public static string Name { get; } =
            $"{nameof(Alkahest)} {nameof(Extractor)}";

        static readonly Log _log = new Log(typeof(Application));

        static readonly IList<ICommand> _commands = new List<ICommand>
        {
            new DecryptCommand(),
            new DumpJsonCommand(),
            new DumpXmlCommand()
        };

        static string _output;

        static bool HandleArguments(ref string[] args)
        {
            var asm = Assembly.GetExecutingAssembly();
            var name = asm.GetName().Name;
            var version = false;
            var help = false;
            var set = new OptionSet
            {
                $"This is {name}, part of the {nameof(Alkahest)} project.",
                "",
                "Usage:",
                "",
                $"  {name} [options...] [--] <command> <arguments>",
                "",
                "General",
                {
                    "h|?|help",
                    "Print version and exit.",
                    h => help = h != null
                },
                {
                    "v|version",
                    "Print help and exit.",
                    v => version = v != null
                },
                {
                    "o|output",
                    "Specify output file or directory.",
                    o => _output = o
                },
                "",
                "Commands:",
                ""
            };

            foreach (var cmd in _commands)
                set.Add($"  {cmd.Syntax}: {cmd.Description}");

            args = set.Parse(args).ToArray();

            if (version)
            {
                Console.WriteLine("{0} {1}", name,
                    asm.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                    .InformationalVersion);
                return false;
            }

            if (help)
            {
                set.WriteOptionDescriptions(Console.Out);
                return false;
            }

            return true;
        }

        public static int Run(string[] args)
        {
            try
            {
                if (!HandleArguments(ref args))
                    return 0;
            }
            catch (OptionException e)
            {
                Console.Error.WriteLine(e.Message);
                return 1;
            }

            if (args.Length == 0)
            {
                Console.Error.WriteLine("Expected command name.");
                return 1;
            }

            var cmdName = args[0].ToLowerInvariant();
            var cmd = _commands.SingleOrDefault(x => x.Name == cmdName);

            if (cmd == null)
            {
                Console.Error.WriteLine("Invalid command name: {0}", cmdName);
                return 1;
            }

            args = args.Slice(1, args.Length - 1);

            if (args.Length != cmd.RequiredArguments)
            {
                Console.Error.WriteLine("Expected exactly {0} arguments.",
                    cmd.RequiredArguments);
                return 1;
            }

            Log.Level = LogLevel.Debug;
            Log.TimestampFormat = "HH:mm:ss:fff";

            var color = Console.ForegroundColor;

            Log.Loggers.Add(new ConsoleLogger(false,
                color, color, color, color, color));

            if (!Debugger.IsAttached)
                AppDomain.CurrentDomain.UnhandledException += UnhandledException;

            cmd.Run(_output, args);

            return 0;
        }

        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        static void UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            _log.Error("Unhandled exception:");
            _log.Error(args.ExceptionObject.ToString());
            _log.Error("{0} will terminate", Name);

            Environment.Exit(1);
        }
    }
}
