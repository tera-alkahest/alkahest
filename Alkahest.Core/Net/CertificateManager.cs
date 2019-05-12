using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Alkahest.Core.Logging;

namespace Alkahest.Core.Net
{
    public sealed class CertificateManager : IDisposable
    {
        const X509KeyStorageFlags StorageFlags =
            X509KeyStorageFlags.MachineKeySet |
            X509KeyStorageFlags.Exportable;

        static readonly string _exePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.System),
            "netsh.exe");

        static readonly Log _log = new Log(typeof(CertificateManager));

        readonly X509Store _store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);

        readonly int _port;

        bool _disposed;

        public X509Certificate2 Signature { get; }

        public X509Certificate2 Encryption { get; }

        public CertificateManager(int port)
        {
            if (port < IPEndPoint.MinPort || port > IPEndPoint.MaxPort)
                throw new ArgumentOutOfRangeException(nameof(port));

            _port = port;

            static byte[] ReadCertificate(string name)
            {
                var asm = Assembly.GetExecutingAssembly();
                using var stream = asm.GetManifestResourceStream($"{name}.pfx");
                using var memory = new MemoryStream((int)stream.Length);

                stream.CopyTo(memory);

                return memory.ToArray();
            }

            Signature = new X509Certificate2(
                ReadCertificate($"{nameof(Alkahest)}CA"), (string)null, StorageFlags);
            Encryption = new X509Certificate2(
                ReadCertificate($"{nameof(Alkahest)}"), (string)null, StorageFlags);

            _store.Open(OpenFlags.ReadWrite);
            AddKeys();

            NetShellRemove(false);
            InvokeNetShell($"add sslcert ipport=0.0.0.0:{port} certstorename={StoreName.Root} " +
                $"certhash={Encryption.Thumbprint} appid={{{Guid.NewGuid()}}}", true);

            _log.Basic("Installed root certificates");
        }

        ~CertificateManager()
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

            NetShellRemove(true);
            RemoveKeys();
            _store.Dispose();

            _log.Basic("Uninstalled root certificates");
        }

        void AddKeys()
        {
            _store.Add(Signature);
            _store.Add(Encryption);
        }

        void RemoveKeys()
        {
            _store.Remove(Encryption);
            _store.Remove(Signature);
        }

        void NetShellRemove(bool check)
        {
            InvokeNetShell($"del sslcert ipport=0.0.0.0:{_port}", check);
        }

        static void InvokeNetShell(string arguments, bool check)
        {
            using var proc = new Process()
            {
                StartInfo =
                {
                    Arguments = $"http {arguments}",
                    FileName = _exePath,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                },
            };

            proc.Start();
            proc.WaitForExit();

            if (check && proc.ExitCode != 0)
                throw new Exception(); // FIXME
        }
    }
}
