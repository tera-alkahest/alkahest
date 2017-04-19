using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.Remoting;
using System.Security;
using EasyHook;
using Mono.Options;
using Alkahest.Core.Logging;
using Alkahest.Core.Logging.Loggers;
using System.Linq;
using System.IO;

namespace Alkahest.Scanner
{
    static class Application
    {
        public static string Name { get; } =
            $"{nameof(Alkahest)} {nameof(Scanner)}";

        static readonly Log _log = new Log(typeof(Application));

        static string _output = "Scan";

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
                    "Specify output directory.",
                    o => _output = o
                }
            };

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

            Log.Level = LogLevel.Debug;
            Log.TimestampFormat = "HH:mm:ss:fff";

            var color = Console.ForegroundColor;

            Log.Loggers.Add(new ConsoleLogger(false,
                color, color, color, color, color));

            if (!Debugger.IsAttached)
                AppDomain.CurrentDomain.UnhandledException += UnhandledException;

            var procs = Process.GetProcessesByName("TERA");

            if (procs.Length == 0)
            {
                _log.Error("Could not find the TERA process");
                return 1;
            }

            if (procs.Length != 1)
            {
                _log.Error("Found multiple TERA processes; please run only one");
                return 1;
            }

            var proc = procs[0];
            var fileName = Path.GetFileName(proc.MainModule.FileName);

            if (fileName != "TERA.exe")
            {
                _log.Error("Process {0} ({1}) has unexpected file name: {2}",
                    proc.ProcessName, proc.Id, fileName);
                return 1;
            }

            _log.Basic("Injecting process {0} ({1})", proc.ProcessName, proc.Id);

            var chanName = IpcChannel.Create();
            var loc = Assembly.GetExecutingAssembly().Location;

            RemoteHooking.Inject(proc.Id, InjectionOptions.DoNotRequireStrongName,
                loc, loc, chanName);

            var chan = IpcChannel.Connect(chanName);

            chan.Wait();

            Directory.CreateDirectory(_output);

            if (chan.Version1 != null && chan.Version2 != null)
            {
                var verPath = Path.Combine(_output, "ver.txt");

                File.WriteAllLines(verPath, new[]
                {
                    chan.Version1.ToString(),
                    chan.Version2.ToString()
                });

                _log.Basic("Wrote client versions to {0}", verPath);
            }

            if (chan.DataCenterKey != null)
            {
                var keyPath = Path.Combine(_output, "key.txt");

                File.WriteAllLines(keyPath, new[]
                {
                    string.Join(" ", chan.DataCenterKey.Select(x => x.ToString("X2")))
                });

                _log.Basic("Wrote data center key to {0}", keyPath);
            }

            if (chan.DataCenterIV != null)
            {
                var ivPath = Path.Combine(_output, "iv.txt");

                File.WriteAllLines(ivPath, new[]
                {
                    string.Join(" ", chan.DataCenterIV.Select(x => x.ToString("X2")))
                });

                _log.Basic("Wrote data center IV to {0}", ivPath);
            }

            if (chan.GameMessages != null)
            {
                var opcPath = Path.Combine(_output, "opc.txt");

                File.WriteAllLines(opcPath, chan.GameMessages.Select(
                    x => $"{x.Value} = {x.Key}").ToArray());

                _log.Basic("Wrote opcodes to {0}", opcPath);
            }

            if (chan.SystemMessages != null)
            {
                var smtPath = Path.Combine(_output, "smt.txt");

                File.WriteAllLines(smtPath, chan.SystemMessages.Select(
                    x => $"{x.Value} = {x.Key}").ToArray());

                _log.Basic("Wrote system messages to {0}", smtPath);
            }

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
