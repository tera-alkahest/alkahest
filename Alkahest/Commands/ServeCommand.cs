using Alkahest.Core;
using Alkahest.Core.Data;
using Alkahest.Core.Logging;
using Alkahest.Core.Net;
using Alkahest.Core.Net.Game;
using Alkahest.Core.Net.Game.Serialization;
using Alkahest.Core.Plugins;
using Mono.Options;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime;
using System.Threading;
using Vanara.PInvoke;

namespace Alkahest.Commands
{
    sealed class ServeCommand : AlkahestCommand
    {
        static readonly Log _log = new Log(typeof(ServeCommand));

        public override GCLatencyMode LatencyMode => GCLatencyMode.SustainedLowLatency;

        bool _cleanup;

        readonly ManualResetEventSlim _running = new ManualResetEventSlim();

        readonly ManualResetEventSlim _exited = new ManualResetEventSlim();

        public ServeCommand()
            : base("Server", "serve", "Run the proxy server (default command)")
        {
            Options = new OptionSet
            {
                $"Usage: {Program.Name} {Name} [OPTIONS]",
                string.Empty,
                "Available options:",
                string.Empty,
                {
                    "c|cleanup",
                    $"Enable/disable only doing cleanup (defaults to `{_cleanup}`)",
                    c => _cleanup = c != null
                },
            };
        }

        protected override int Invoke(string[] args)
        {
            if (_cleanup)
            {
                var sls = ServerListParameters.Uris[Configuration.Region];
                var slsPort = Configuration.ServerListPort;

                if (slsPort == 0)
                    slsPort = sls.Port;

                using var hostsMgr = Configuration.AdjustHostsFile ? new HostsFileManager() : null;
                using var certMgr = Configuration.AdjustCertificateStore && sls.Scheme == Uri.UriSchemeHttps ?
                    new CertificateManager(slsPort) : null;

                hostsMgr?.RemoveEntry(sls.Host, Configuration.ServerListBaseAddress);

                return 0;
            }

            if (Configuration.Loggers.Contains(FileLogger.Name))
                Log.Loggers.Add(new FileLogger(Configuration.LogDirectory, Configuration.LogFileNameFormat));

            try
            {
                AppDomain.CurrentDomain.ProcessExit += ProcessExit;
                Console.CancelKeyPress += CancelKeyPress;
                ConsoleUtility.AddConsoleEventHandler(ConsoleEvent);

                _log.Basic("Proxy server starting...");

                var region = Configuration.Region;
                var sls = ServerListParameters.Uris[region];
                var slsPort = Configuration.ServerListPort;

                if (slsPort == 0)
                    slsPort = sls.Port;

                using var hostsMgr = Configuration.AdjustHostsFile ? new HostsFileManager() : null;
                using var certMgr = Configuration.AdjustCertificateStore && sls.Scheme == Uri.UriSchemeHttps ?
                    new CertificateManager(slsPort) : null;

                var slsHost = sls.Host;
                var slsAddress = Configuration.ServerListBaseAddress;

                hostsMgr?.RemoveEntry(slsHost, slsAddress);

                var real = Dns.GetHostEntry(slsHost).AddressList[0];

                _log.Basic("Resolved official server list address: {0} -> {1}", slsHost, real);

                hostsMgr?.AddEntry(slsHost, slsAddress);

                using var slsProxy = new ServerListProxy(new ServerListParameters(real,
                    Configuration.ServerListBaseAddress, slsPort, Configuration.GameBaseAddress,
                    Configuration.GameBasePort, region, Configuration.ServerListTimeout,
                    Configuration.ServerListRetries));

                if (Configuration.ServerListEnabled)
                    slsProxy.Start();

                var pool = new ObjectPool<SocketAsyncEventArgs>(() => new SocketAsyncEventArgs(),
                    x => x.Reset(), Configuration.PoolLimit != 0 ? (int?)Configuration.PoolLimit : null);
                var version = MessageTable.Versions[region];
                var proc = new PacketProcessor(new CompilerPacketSerializer(region,
                    new GameMessageTable(version), new SystemMessageTable(version)));
                var proxies = slsProxy.Servers.Select(x => new GameProxy(x, pool, proc,
                    Configuration.GameBacklog, Configuration.GameTimeout)
                {
                    MaxClients = Configuration.GameMaxClients,
                }).ToArray();

                foreach (var proxy in proxies)
                    proxy.Start();

                var path = Path.ChangeExtension(Path.Combine(Configuration.AssetDirectory,
                    DataCenter.FileNames[region]), ".dec");
                var loader = new PluginLoader(new PluginContext(File.Exists(path) ?
                    new DataCenter(path) : new DataCenter()), Configuration.PluginDirectory,
                    Configuration.PluginPattern, Configuration.DisablePlugins);

                loader.Start(proxies);

                _log.Basic("Proxy server started");

                _running.Wait();

                _log.Basic("Proxy server stopping...");

                loader.Stop(proxies);

                foreach (var proxy in proxies)
                    proxy.Dispose();

                _exited.Set();

                _log.Basic("Proxy server stopped");
            }
            finally
            {
                ConsoleUtility.RemoveConsoleEventHandler(ConsoleEvent);
                Console.CancelKeyPress -= CancelKeyPress;
                AppDomain.CurrentDomain.ProcessExit -= ProcessExit;
            }

            return 0;
        }

        void Stop()
        {
            _running.Set();
            _exited.Wait();
        }

        bool ConsoleEvent(Kernel32.CTRL_EVENT @event)
        {
            if (@event == Kernel32.CTRL_EVENT.CTRL_CLOSE_EVENT)
                Stop();

            return false;
        }

        void CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;

            Stop();
        }

        void ProcessExit(object sender, EventArgs e)
        {
            Stop();
        }
    }
}
