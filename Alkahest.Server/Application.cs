using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Threading;
using Alkahest.Core;
using Alkahest.Core.Logging;
using Alkahest.Core.Logging.Loggers;
using Alkahest.Core.Net;
using Alkahest.Core.Net.Protocol;
using Alkahest.Core.Net.Protocol.Logging;
using Alkahest.Core.Plugins;

namespace Alkahest.Server
{
    static class Application
    {
        public static string Name { get; }

        static readonly ManualResetEventSlim _event =
            new ManualResetEventSlim();

        static readonly Log _log = new Log(typeof(Application));

        static Application()
        {
            Name = $"{nameof(Alkahest)} {nameof(Server)}";
        }

        public static int Run(string[] args)
        {
            Console.CancelKeyPress += CancelKeyPress;

            Log.Level = Configuration.LogLevel;
            Log.TimestampFormat = Configuration.LogTimestampFormat;

            foreach (var src in Configuration.DiscardLogSources)
                Log.DiscardSources.Add(src);

            if (Configuration.Loggers.Contains(ConsoleLogger.Name))
                Log.Loggers.Add(new ConsoleLogger(
                    Configuration.EnableColors, Configuration.ErrorColor,
                    Configuration.BasicColor, Configuration.InfoColor,
                    Configuration.DebugColor));

            if (Configuration.Loggers.Contains(FileLogger.Name))
                Log.Loggers.Add(new FileLogger(
                    Configuration.LogDirectory, Configuration.LogFileNameFormat));

            if (!Debugger.IsAttached)
                AppDomain.CurrentDomain.UnhandledException += UnhandledException;

            _log.Basic("Starting {0}...", Name);

            using (var hosts = Configuration.AdjustHostsFile ? new HostsFileManager() : null)
            {
                var region = Configuration.Region;
                var slsHost = ServerListParameters.GetHost(region);
                var slsAddress = Configuration.ServerListAddress;

                hosts?.RemoveEntry(slsHost, slsAddress);

                var real = Dns.GetHostEntry(slsHost).AddressList[0];

                hosts?.AddEntry(slsHost, slsAddress);

                var slsParams = new ServerListParameters(real,
                    Configuration.ServerListAddress, Configuration.GameAddress,
                    region, Configuration.ServerListTimeout,
                    Configuration.ServerListRetries);

                using (var slsProxy = new ServerListProxy(slsParams))
                {
                    var pool = new ObjectPool<SocketAsyncEventArgs>(
                        () => new SocketAsyncEventArgs(), x => x.Reset(),
                        Configuration.PoolLimit != 0 ? (int?)Configuration.PoolLimit : null);

                    using (var writer = Configuration.EnablePacketLogs ?
                        new PacketLogWriter(region, Configuration.PacketLogDirectory,
                            Configuration.PacketLogFileNameFormat,
                            Configuration.CompressPacketLogs) : null)
                    {
                        var proc = new PacketProcessor(new PacketSerializer(
                            new OpCodeTable(true, region),
                            new OpCodeTable(false, region)), writer,
                            Configuration.GamePacketRoundtrips);
                        var proxies = slsProxy.Servers.Select(x => new GameProxy(
                            x, pool, proc, Configuration.GameBacklog,
                            Configuration.GameTimeout)
                        {
                            MaxClients = Configuration.GameMaxClients
                        }).ToArray();
                        var loader = new PluginLoader(Configuration.PluginDirectory,
                            Configuration.PluginPattern, Configuration.DisablePlugins);

                        loader.Start(proxies);

                        _log.Basic("{0} started", Name);

                        _event.Wait();

                        _log.Basic("{0} shutting down...", Name);

                        loader.Stop(proxies);

                        foreach (var proxy in proxies)
                            proxy.Dispose();
                    }
                }
            }

            return 0;
        }

        static void CancelKeyPress(object sender, ConsoleCancelEventArgs args)
        {
            args.Cancel = true;
            _event.Set();
        }

        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        static void UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            _log.Error("Unhandled exception:");
            _log.Error(args.ExceptionObject.ToString());
            _log.Error("{0} will terminate", Name);

            _event.Wait();
        }
    }
}
