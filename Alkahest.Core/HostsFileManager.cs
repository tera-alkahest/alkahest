using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Alkahest.Core.Logging;

namespace Alkahest.Core
{
    public sealed class HostsFileManager : IDisposable
    {
        const string HostsFileRegistryPath = @"SYSTEM\CurrentControlSet\services\Tcpip\Parameters";

        const string HostsFileRegistryKey = "DataBasePath";

        const string HostsFileName = "hosts";

        static readonly Log _log = new Log(typeof(HostsFileManager));

        readonly string _hosts;

        readonly List<string> _entries = new List<string>();

        bool _disposed;

        public HostsFileManager()
        {
            using (var key = Registry.LocalMachine.OpenSubKey(HostsFileRegistryPath))
                _hosts = Path.Combine((string)key.GetValue(HostsFileRegistryKey), HostsFileName);
        }

        ~HostsFileManager()
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

            File.WriteAllLines(_hosts, File.ReadAllLines(_hosts).Where(x => !_entries.Contains(x)));

            _log.Basic("Removed all hosts entries");
        }

        string MakeEntry(string host, IPAddress destination)
        {
            return $"{destination.ToString()} {host}";
        }

        public void AddEntry(string host, IPAddress destination)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            var entry = MakeEntry(host, destination);

            _entries.Add(entry);
            File.AppendAllLines(_hosts, new[] { entry });

            _log.Basic("Added hosts entry: {0} -> {1}", host, destination);
        }

        public void RemoveEntry(string host, IPAddress destination)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            var entry = MakeEntry(host, destination);

            _entries.Remove(entry);
            File.WriteAllLines(_hosts, File.ReadAllLines(_hosts).Where(x => x != entry));

            _log.Basic("Removed hosts entry: {0} -> {1}", host, destination);
        }
    }
}
