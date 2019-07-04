using Alkahest.Core.Logging;
using Alkahest.Core.Net.Game.Logging;
using Alkahest.Core.Plugins;
using System;
using System.Linq;

namespace Alkahest.Plugins.PacketLogger
{
    public sealed class PacketLoggerPlugin : IPlugin
    {
        public string Name => "packet-logger";

        static readonly Log _log = new Log(typeof(PacketLoggerPlugin));

        readonly PluginContext _context;

        PacketLogWriter _writer;

#pragma warning disable IDE0051 // Remove unused private members
        PacketLoggerPlugin(PluginContext context)
#pragma warning restore IDE0051 // Remove unused private members
        {
            _context = context;
        }

        public void Start()
        {
            var serializer = _context.Serializer;

            _writer = new PacketLogWriter(serializer.Region, serializer.GameMessages,
                serializer.SystemMessages, _context.Proxies.Select(x => x.Info).ToArray(),
                Configuration.LogDirectory, Configuration.LogFileNameFormat,
                Configuration.CompressLogs);

            _context.Dispatch.AddHandler((client, direction, code, packet, flags) =>
            {
                _writer.Write(new PacketLogEntry(DateTime.Now, client.Proxy.Info.Id, direction,
                    code, packet.Payload));

                return true;
            }, new PacketFilter(long.MinValue).WithSilenced(null));

            _log.Basic("Packet logger plugin started");
        }

        public void Stop()
        {
            _writer.Dispose();

            _log.Basic("Packet logger plugin stopped");
        }
    }
}
