using System;
using System.Linq;
using Alkahest.Core;
using Alkahest.Core.Logging;
using Alkahest.Core.Net;
using Alkahest.Core.Net.Protocol;
using Alkahest.Core.Net.Protocol.Logging;
using Alkahest.Core.Plugins;

namespace Alkahest.Plugins.PacketLogger
{
    public sealed class PacketLoggerPlugin : IPlugin
    {
        public string Name { get; } = "packet-logger";

        static readonly Log _log = new Log(typeof(PacketLoggerPlugin));

        PacketLogWriter _writer;

        public void Start(GameProxy[] proxies)
        {
            _writer = new PacketLogWriter(
                proxies.First().Processor.Serializer.Messages,
                proxies.Select(x => x.Info).ToArray(),
                Configuration.LogDirectory, Configuration.LogFileNameFormat,
                Configuration.CompressLogs);

            foreach (var proxy in proxies)
                proxy.Processor.AddRawHandler(PacketLogHandler);

            _log.Basic("Packet logger plugin started");
        }

        public void Stop(GameProxy[] proxies)
        {
            foreach (var proxy in proxies)
                proxy.Processor.RemoveRawHandler(PacketLogHandler);

            _writer.Dispose();

            _log.Basic("Packet logger plugin stopped");
        }

        private bool PacketLogHandler(GameClient client, Direction direction,
            RawPacket packet)
        {
            _writer.Write(new PacketLogEntry(
                DateTime.Now, client.Proxy.Info.Id, direction,
                client.Proxy.Processor.Serializer.Messages.Game.NameToOpCode[packet.OpCode],
                packet.Payload));

            return true;
        }
    }
}
