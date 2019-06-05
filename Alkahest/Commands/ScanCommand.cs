using Alkahest.Core.Logging;
using Alkahest.Scanner;
using EasyHook;
using Mono.Options;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Alkahest.Commands
{
    sealed class ScanCommand : AlkahestCommand
    {
        static readonly Log _log = new Log(typeof(ScanCommand));

        string _output = "Scan";

        public ScanCommand()
            : base("Scanner", "scan", "Scan a running client for useful data")
        {
            Options = new OptionSet
            {
                $"Usage: {Program.Name} {Name} [OPTIONS]",
                string.Empty,
                "Available options:",
                string.Empty,
                {
                    "o|output=",
                    $"Specify output directory (defaults to `{_output}`)",
                    o => _output = o
                },
            };
        }

        protected override int Invoke(string[] args)
        {
            var procs = Process.GetProcessesByName("TERA");

            if (procs.Length == 0)
            {
                _log.Error("Could not find the TERA process");
                return 1;
            }

            if (procs.Length != 1)
                _log.Warning("Found multiple TERA processes; using the first one");

            var proc = procs[0];
            var fileName = Path.GetFileName(proc.MainModule.FileName);

            if (fileName != "TERA.exe")
                _log.Warning("Process {0} ({1}) has unexpected file name: {2}", proc.ProcessName,
                    proc.Id, fileName);

            var chanName = IpcChannel.Create();

            _log.Info("Connecting to IPC channel {0}...", chanName);

            var chan = IpcChannel.Connect(chanName);

            chan.SetOutputDirectory(_output);

            _log.Basic("Injecting process {0} ({1})...", proc.ProcessName, proc.Id);

            var loc = Assembly.GetExecutingAssembly().Location;

            RemoteHooking.Inject(proc.Id, InjectionOptions.DoNotRequireStrongName, loc, loc, chanName);

            chan.Wait();

            return 0;
        }
    }
}
