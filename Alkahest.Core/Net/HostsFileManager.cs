using Alkahest.Core.Logging;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

namespace Alkahest.Core.Net
{
    public sealed class HostsFileManager : IDisposable
    {
        const string HostsFileRegistryPath = @"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters";

        const string HostsFileRegistryKey = "DataBasePath";

        const string HostsFileName = "hosts";

        const string MutexName = @"Global\" + nameof(Alkahest) + "-" + nameof(HostsFileManager);

        static readonly Log _log = new Log(typeof(HostsFileManager));

        static readonly Mutex _mutex = new Mutex(false, MutexName);

        static readonly string _hostsPath;

        readonly List<string> _entries = new List<string>();

        bool _disposed;

        static HostsFileManager()
        {
            using var key = Registry.LocalMachine.OpenSubKey(HostsFileRegistryPath);

            _hostsPath = Path.Combine((string)key.GetValue(HostsFileRegistryKey), HostsFileName);
        }

        static void RunLocked(Action action)
        {
            try
            {
                try
                {
                    _mutex.WaitOne();
                }
                catch (AbandonedMutexException)
                {
                }

                action();
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
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

            RunLocked(() => File.WriteAllLines(_hostsPath,
                File.ReadAllLines(_hostsPath).Where(x => !_entries.Contains(x))));

            _log.Basic("Removed all hosts entries");
        }

        string MakeEntry(string host, IPAddress destination)
        {
            if (host == null)
                throw new ArgumentNullException(nameof(host));

            if (Uri.CheckHostName(host) != UriHostNameType.Dns)
                throw new ArgumentException("Invalid host name.", nameof(host));

            if (destination == null)
                throw new ArgumentNullException(nameof(destination));

            return $"{destination} {host} # {nameof(Alkahest)}";
        }

        public void AddEntry(string host, IPAddress destination)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            var entry = MakeEntry(host, destination);

            _entries.Add(entry);
            RunLocked(() => File.AppendAllLines(_hostsPath, new[] { entry }));

            _log.Basic("Added hosts entry: {0} -> {1}", host, destination);
        }

        public void RemoveEntry(string host, IPAddress destination)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            var entry = MakeEntry(host, destination);

            _entries.Remove(entry);
            RunLocked(() => File.WriteAllLines(_hostsPath,
                File.ReadAllLines(_hostsPath).Where(x => x != entry)));

            _log.Basic("Removed hosts entry: {0} -> {1}", host, destination);
        }
    }
}
