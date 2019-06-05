using Alkahest.Core.Logging;
using EasyHook;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;
using System.Threading;

namespace Alkahest.Scanner
{
    sealed class IpcChannel : MarshalByRefObject
    {
        public static string Create()
        {
            string name = null;

            RemoteHooking.IpcCreateServer<IpcChannel>(ref name, WellKnownObjectMode.Singleton);

            return name;
        }

        public static IpcChannel Connect(string name)
        {
            return RemoteHooking.IpcConnectClient<IpcChannel>(name);
        }

        static readonly Log _log = new Log(typeof(IpcChannel));

        readonly AutoResetEvent _event = new AutoResetEvent(false);

        string _output;

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public void SetOutputDirectory(string directory)
        {
            Directory.CreateDirectory(directory);

            _output = directory;
        }

        public void Wait()
        {
            _event.WaitOne();
        }

        public void Signal()
        {
            _event.Set();
        }

        public void LogBasic(string format, params object[] args)
        {
            _log.Basic(format, args);
        }

        public void LogError(string format, params object[] args)
        {
            _log.Error(format, args);
        }

        public void WriteVersions(uint version1, uint version2)
        {
            var verPath = Path.Combine(_output, "Versions.txt");

            File.WriteAllLines(verPath, new[]
            {
                version1.ToString(),
                version2.ToString(),
            });

            _log.Basic("Wrote client versions to {0}", verPath);
        }

        public void WriteDataCenterKey(IReadOnlyList<byte> key)
        {
            var keyPath = Path.Combine(_output, "DataCenterKey.txt");

            File.WriteAllLines(keyPath, new[]
            {
                string.Join(" ", key.Select(x => x.ToString("X2"))),
            });

            _log.Basic("Wrote data center key to {0}", keyPath);
        }

        public void WriteDataCenterIV(IReadOnlyList<byte> iv)
        {
            var ivPath = Path.Combine(_output, "DataCenterIV.txt");

            File.WriteAllLines(ivPath, new[]
            {
                string.Join(" ", iv.Select(x => x.ToString("X2"))),
            });

            _log.Basic("Wrote data center IV to {0}", ivPath);
        }

        public void WriteGameMessages(IReadOnlyList<Tuple<ushort, string>> messages)
        {
            var gmtPath = Path.Combine(_output, "GameMessageTable.txt");

            File.WriteAllLines(gmtPath, messages.Select(x => $"{x.Item2} = {x.Item1}"));

            _log.Basic("Wrote game messages to {0}", gmtPath);
        }

        public void WriteSystemMessages(IReadOnlyList<Tuple<ushort, string>> messages)
        {
            var smtPath = Path.Combine(_output, "SystemMessageTable.txt");

            File.WriteAllLines(smtPath, messages.Select(x => $"{x.Item2} = {x.Item1}"));

            _log.Basic("Wrote system messages to {0}", smtPath);
        }
    }
}
