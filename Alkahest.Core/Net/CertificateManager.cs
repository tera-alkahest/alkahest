using Alkahest.Core.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

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
                using var stream = asm.GetManifestResourceStream(name);
                using var memory = new MemoryStream((int)stream.Length);

                stream.CopyTo(memory);

                return memory.ToArray();
            }

            Signature = new X509Certificate2(ReadCertificate($"{nameof(Alkahest)}CA.crt"),
                (string)null, StorageFlags);
            Encryption = new X509Certificate2(ReadCertificate($"{nameof(Alkahest)}.pfx"),
                (string)null, StorageFlags);
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

            NetShellRemove();

            _store.Open(OpenFlags.ReadWrite);
            RemoveKeys();
            _store.Dispose();

            _log.Basic("Uninstalled root certificates");
        }

        public void Activate()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);

            _store.Open(OpenFlags.ReadWrite);
            AddKeys();
            _store.Close();

            NetShellRemove();
            InvokeNetShell("add", $"certstorename={StoreName.Root} certhash={Encryption.Thumbprint} " +
                $"appid={{{Guid.NewGuid()}}}", true);

            _log.Basic("Installed root certificates");
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

        void NetShellRemove()
        {
            InvokeNetShell("del", string.Empty, false);
        }

        void InvokeNetShell(string command, string arguments, bool check)
        {
            using var proc = new Process
            {
                StartInfo =
                {
                    Arguments = $"http {command} sslcert ipport=0.0.0.0:{_port} {arguments}",
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
                throw new InvalidOperationException(
                    $"{_exePath} failed with exit code {proc.ExitCode}:{Environment.NewLine}" +
                    proc.StandardOutput.ReadToEnd().Trim());
        }
    }
}
