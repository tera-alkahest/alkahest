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
using Alkahest.Core.Net.Protocol.Serializers;
using Alkahest.Core.Plugins;

namespace Alkahest.Server
{
    static class Application
    {
        public static string Name { get; } =
            $"{nameof(Alkahest)} {nameof(Server)}";

        static readonly ManualResetEventSlim _runningEvent =
            new ManualResetEventSlim();

        static readonly ManualResetEventSlim _exitEvent =
            new ManualResetEventSlim();

        static readonly Log _log = new Log(typeof(Application));

        public static int Run(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += ProcessExit;
            Console.CancelKeyPress += CancelKeyPress;
            ConsoleUtility.AddConsoleEventHandler(ConsoleEvent);

            Log.Level = Configuration.LogLevel;
            Log.TimestampFormat = Configuration.LogTimestampFormat;

            foreach (var src in Configuration.DiscardLogSources)
                Log.DiscardSources.Add(src);

            if (Configuration.Loggers.Contains(ConsoleLogger.Name))
                Log.Loggers.Add(new ConsoleLogger(
                    Configuration.EnableColors, Configuration.ErrorColor,
                    Configuration.WarningColor, Configuration.BasicColor,
                    Configuration.InfoColor, Configuration.DebugColor));

            if (Configuration.Loggers.Contains(FileLogger.Name))
                Log.Loggers.Add(new FileLogger(
                    Configuration.LogDirectory, Configuration.LogFileNameFormat));

            if (!Debugger.IsAttached)
                AppDomain.CurrentDomain.UnhandledException += UnhandledException;

            _log.Basic("Starting {0}...", Name);

            using (var hosts = Configuration.AdjustHostsFile ? new HostsFileManager() : null)
            {
                var region = Configuration.Region;
                var slsHost = ServerListParameters.GetUri(region).Host;
                var slsAddress = Configuration.ServerListAddress;

                hosts?.RemoveEntry(slsHost, slsAddress);

                var real = Dns.GetHostEntry(slsHost).AddressList[0];

                hosts?.AddEntry(slsHost, slsAddress);

                var slsParams = new ServerListParameters(real,
                    Configuration.ServerListAddress, Configuration.GameAddress,
                    Configuration.GameStartingPort, region,
                    Configuration.ServerListTimeout, Configuration.ServerListRetries);

                using (var slsProxy = new ServerListProxy(slsParams))
                {
                    var pool = new ObjectPool<SocketAsyncEventArgs>(
                        () => new SocketAsyncEventArgs(), x => x.Reset(),
                        Configuration.PoolLimit != 0 ? (int?)Configuration.PoolLimit : null);
                    var servers = slsProxy.Servers.ToArray();

                    using (var writer = Configuration.EnablePacketLogs ?
                        new PacketLogWriter(region, servers,
                            Configuration.PacketLogDirectory,
                            Configuration.PacketLogFileNameFormat,
                            Configuration.CompressPacketLogs) : null)
                    {
                        var ver = OpCodeTable.Versions[region];
                        var proc = new PacketProcessor(
                            new CompilerPacketSerializer(
                                new OpCodeTable(true, ver),
                                new OpCodeTable(false, ver)), writer);
                        var proxies = servers.Select(x => new GameProxy(x,
                            pool, proc, Configuration.GameBacklog,
                            Configuration.GameTimeout)
                        {
                            MaxClients = Configuration.GameMaxClients
                        }).ToArray();
                        var loader = new PluginLoader(Configuration.PluginDirectory,
                            Configuration.PluginPattern, Configuration.DisablePlugins);

                        loader.Start(proxies);

                        _log.Basic("{0} started", Name);

                        _runningEvent.Wait();

                        _log.Basic("{0} shutting down...", Name);

                        loader.Stop(proxies);

                        foreach (var proxy in proxies)
                            proxy.Dispose();
                    }
                }
            }

            _exitEvent.Set();

            return 0;
        }

        static void ProcessExit(object sender, EventArgs e)
        {
            _runningEvent.Set();
            _exitEvent.Wait();
        }

        static void CancelKeyPress(object sender, ConsoleCancelEventArgs args)
        {
            args.Cancel = true;

            _runningEvent.Set();
            _exitEvent.Wait();
        }

        static bool ConsoleEvent(int @event)
        {
            if (@event == ConsoleUtility.CloseEvent)
            {
                _runningEvent.Set();
                _exitEvent.Wait();
            }

            return false;
        }

        [HandleProcessCorruptedStateExceptions]
        [SecurityCritical]
        static void UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            _log.Error("Unhandled exception:");
            _log.Error(args.ExceptionObject.ToString());
            _log.Error("{0} will terminate", Name);

            Environment.Exit(1);
        }
    }
}
