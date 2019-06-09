using Alkahest.Core;
using Alkahest.Core.Logging;
using Alkahest.Core.Net.Game;
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
            var serializer = _context.Proxies.First().Processor.Serializer;

            _writer = new PacketLogWriter(serializer.Region, serializer.GameMessages,
                serializer.SystemMessages, _context.Proxies.Select(x => x.Info).ToArray(),
                Configuration.LogDirectory, Configuration.LogFileNameFormat,
                Configuration.CompressLogs);

            foreach (var proxy in _context.Proxies)
                proxy.Processor.AddRawHandler(PacketLogHandler);

            _log.Basic("Packet logger plugin started");
        }

        public void Stop()
        {
            foreach (var proxy in _context.Proxies)
                proxy.Processor.RemoveRawHandler(PacketLogHandler);

            _writer.Dispose();

            _log.Basic("Packet logger plugin stopped");
        }

        bool PacketLogHandler(GameClient client, Direction direction, RawPacket packet)
        {
            _writer.Write(new PacketLogEntry(DateTime.Now, client.Proxy.Info.Id, direction,
                client.Proxy.Processor.Serializer.GameMessages.NameToCode[packet.Name], packet.Payload));

            return true;
        }
    }
}
