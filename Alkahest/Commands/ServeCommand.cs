using Alkahest.Core;
using Alkahest.Core.Data;
using Alkahest.Core.Logging;
using Alkahest.Core.Net;
using Alkahest.Core.Net.Game;
using Alkahest.Core.Net.Game.Serialization;
using Alkahest.Core.Plugins;
using Mono.Options;
using System;
using System.Collections.Generic;
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
        sealed class ServerInstance
        {
            public ServerListProxy ServerListProxy { get; }

            public PluginLoader Loader { get; }

            public IEnumerable<GameProxy> Proxies { get; }

            public ServerInstance(ServerListProxy slsProxy, PluginLoader loader,
                IEnumerable<GameProxy> proxies)
            {
                ServerListProxy = slsProxy;
                Loader = loader;
                Proxies = proxies;
            }
        }

        static readonly Log _log = new Log(typeof(ServeCommand));

        public override GCLatencyMode LatencyMode => GCLatencyMode.SustainedLowLatency;

        bool _cleanup;

        readonly ManualResetEventSlim _exiting = new ManualResetEventSlim();

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

        protected override ConsoleLogger CreateConsoleLogger()
        {
            return Configuration.Loggers.Contains(ConsoleLogger.Name) ? new ConsoleLogger(
                true, true, true, Configuration.ColorsEnabled, Configuration.ErrorColor,
                Configuration.WarningColor, Configuration.BasicColor, Configuration.InfoColor,
                Configuration.DebugColor) : null;
        }

        static ServerInstance StartServer(Region region, HostsFileManager hosts)
        {
            _log.Basic("{0} proxy server starting...", region);

            var sls = ServerListParameters.Uris[region];
            var slsPort = Configuration.ServerListPort;

            if (slsPort == 0)
                slsPort = sls.Port;

            var slsHost = sls.Host;
            var slsAddress = Configuration.ServerListBaseAddress;

            hosts?.RemoveEntry(slsHost, slsAddress);

            var real = Dns.GetHostEntry(slsHost).AddressList[0];

            _log.Info("Resolved {0} server list address: {1} -> {2}", region, slsHost, real);

            hosts?.AddEntry(slsHost, slsAddress);

            var slsProxy = new ServerListProxy(new ServerListParameters(real,
                Configuration.ServerListBaseAddress, slsPort, Configuration.GameBaseAddress,
                Configuration.GameBasePort, region, Configuration.ServerListTimeout,
                Configuration.ServerListRetries));

            var pool = new ObjectPool<SocketAsyncEventArgs>(() => new SocketAsyncEventArgs(),
                x => x.Reset(), Configuration.PoolLimit != 0 ? (int?)Configuration.PoolLimit : null);
            var version = DataCenter.ClientVersions[region];
            var proc = new PacketProcessor(new CompilerPacketSerializer(region,
                new GameMessageTable(version), new SystemMessageTable(version)));
            var proxies = slsProxy.Servers.Select(x => new GameProxy(x, pool, proc,
                Configuration.GameBacklog, Configuration.GameTimeout)
            {
                MaxClients = Configuration.GameMaxClients,
            }).ToArray();

            var path = Path.ChangeExtension(Path.Combine(Configuration.AssetDirectory,
                DataCenter.FileNames[region]), DataCenter.UnpackedExtension);
            var dc = !File.Exists(path) ? new DataCenter(version) : new DataCenter(File.OpenRead(path),
                Configuration.DataCenterMode, Configuration.DataCenterStringOptions);
            var loader = new PluginLoader(new PluginContext(region, dc, proxies),
                Configuration.PluginDirectory, Configuration.PluginPattern, Configuration.DisablePlugins);

            loader.Start();

            if (Configuration.ServerListEnabled)
                slsProxy.Start();

            foreach (var proxy in proxies)
                proxy.Start();

            _log.Basic("{0} proxy server started", region);

            return new ServerInstance(slsProxy, loader, proxies);
        }

        static void StopServer(ServerInstance server)
        {
            var region = server.ServerListProxy.Parameters.Region;

            _log.Basic("{0} proxy server stopping...", region);

            foreach (var proxy in server.Proxies)
                proxy.Dispose();

            server.ServerListProxy.Dispose();

            server.Loader.Stop();

            _log.Basic("{0} proxy server stopped", region);
        }

        protected override int Invoke(string[] args)
        {
            using var hosts = Configuration.AdjustHostsFile ? new HostsFileManager() : null;
            using var certs = Configuration.AdjustCertificateStore ? new CertificateManager(443) : null;

            if (_cleanup)
            {
                _log.Basic("Cleaning up proxy server system changes...");

                foreach (var region in Configuration.Regions)
                    hosts?.RemoveEntry(ServerListParameters.Uris[region].Host,
                        Configuration.ServerListBaseAddress);

                return 0;
            }

            if (Configuration.Loggers.Contains(FileLogger.Name))
                Log.Loggers.Add(new FileLogger(true, true, true, Configuration.LogDirectory,
                    Configuration.LogFileNameFormat));

            try
            {
                AppDomain.CurrentDomain.ProcessExit += ProcessExit;
                Console.CancelKeyPress += CancelKeyPress;
                ConsoleUtility.AddConsoleEventHandler(ConsoleEvent);

                certs?.Activate();

                var servers = Configuration.Regions.Select(x => StartServer(x, hosts)).ToArray();

                _log.Basic("{0} server started", nameof(Alkahest));

                _exiting.Wait();

                foreach (var server in servers)
                    StopServer(server);

                _exited.Set();
            }
            finally
            {
                ConsoleUtility.RemoveConsoleEventHandler(ConsoleEvent);
                Console.CancelKeyPress -= CancelKeyPress;
                AppDomain.CurrentDomain.ProcessExit -= ProcessExit;
            }

            _log.Basic("{0} server stopped", nameof(Alkahest));

            return 0;
        }

        void Stop()
        {
            _exiting.Set();
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
