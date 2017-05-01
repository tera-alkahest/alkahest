using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using Alkahest.Core.Logging;

namespace Alkahest.Core.Net
{
    public sealed class NetworkShellManager : IDisposable
    {
        const string ShellName = "netsh.exe";

        const string ContextName = "interface portproxy";

        static readonly Log _log = new Log(typeof(NetworkShellManager));

        static readonly string _exePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.System),
            ShellName);

        readonly List<IPEndPoint> _proxies = new List<IPEndPoint>();

        bool _disposed;

        ~NetworkShellManager()
        {
            RealDispose();
        }

        public void Dispose()
        {
            RealDispose();
            GC.SuppressFinalize(this);
        }

        void RealDispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            foreach (var ep in _proxies)
                InvokeRemovePortProxy(ep);

            _log.Basic("Removed all port proxies");
        }

        static void InvokeNetworkShell(string arguments)
        {
            var proc = new Process()
            {
                StartInfo =
                {
                    Arguments = $"{ContextName} {arguments}",
                    FileName = _exePath,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                }
            };

            using (proc)
            {
                proc.Start();
                proc.WaitForExit();
            }
        }

        static void InvokeAddPortProxy(IPEndPoint source, IPEndPoint destination)
        {
            var sb = new StringBuilder("add v4tov4");

            sb.Append($" listenaddress={source.Address} listenport={source.Port}");
            sb.Append($" connectaddress={destination.Address} connectport={destination.Port}");

            InvokeNetworkShell(sb.ToString());
        }

        public void AddPortProxy(IPEndPoint source, IPEndPoint destination)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            _proxies.Add(source);
            InvokeAddPortProxy(source, destination);

            _log.Basic("Added port proxy: {0} -> {1}", source, destination);
        }

        static void InvokeRemovePortProxy(IPEndPoint source)
        {
            var sb = new StringBuilder("delete v4tov4");

            sb.Append($" listenaddress={source.Address} listenport={source.Port}");

            InvokeNetworkShell(sb.ToString());
        }

        public void RemovePortProxy(IPEndPoint source, IPEndPoint destination)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            _proxies.Remove(source);
            InvokeRemovePortProxy(source);

            _log.Basic("Removed port proxy: {0} -> {1}", source, destination);
        }
    }
}
