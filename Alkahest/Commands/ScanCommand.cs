using Alkahest.Core.Logging;
using Alkahest.Scanner;
using EasyHook;
using Mono.Options;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
                    "o|output",
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

            _log.Basic("Injecting process {0} ({1})...", proc.ProcessName, proc.Id);

            var chanName = IpcChannel.Create();
            var loc = Assembly.GetExecutingAssembly().Location;

            RemoteHooking.Inject(proc.Id, InjectionOptions.DoNotRequireStrongName, loc, loc, chanName);

            var chan = IpcChannel.Connect(chanName);

            chan.Wait();

            Directory.CreateDirectory(_output);

            if (chan.Version1 != null && chan.Version2 != null)
            {
                var verPath = Path.Combine(_output, "ver.txt");

                File.WriteAllLines(verPath, new[]
                {
                    chan.Version1.ToString(),
                    chan.Version2.ToString(),
                });

                _log.Basic("Wrote client versions to {0}", verPath);
            }

            if (chan.DataCenterKey != null)
            {
                var keyPath = Path.Combine(_output, "key.txt");

                File.WriteAllLines(keyPath, new[]
                {
                    string.Join(" ", chan.DataCenterKey.Select(x => x.ToString("X2"))),
                });

                _log.Basic("Wrote data center key to {0}", keyPath);
            }

            if (chan.DataCenterIV != null)
            {
                var ivPath = Path.Combine(_output, "iv.txt");

                File.WriteAllLines(ivPath, new[]
                {
                    string.Join(" ", chan.DataCenterIV.Select(x => x.ToString("X2"))),
                });

                _log.Basic("Wrote data center IV to {0}", ivPath);
            }

            if (chan.GameMessages != null)
            {
                var opcPath = Path.Combine(_output, "opc.txt");

                File.WriteAllLines(opcPath, chan.GameMessages.Select(x => $"{x.Item2} = {x.Item1}"));

                _log.Basic("Wrote opcodes to {0}", opcPath);
            }

            if (chan.SystemMessages != null)
            {
                var smtPath = Path.Combine(_output, "smt.txt");

                File.WriteAllLines(smtPath, chan.SystemMessages.Select(x => $"{x.Item2} = {x.Item1}"));

                _log.Basic("Wrote system messages to {0}", smtPath);
            }

            return 0;
        }
    }
}
