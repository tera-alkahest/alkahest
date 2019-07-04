using Alkahest.Core.IO;
using Alkahest.Core.Logging;
using Alkahest.Core.Net.Game;
using Alkahest.Core.Net.Game.Packets;
using Alkahest.Core.Plugins;

namespace Alkahest.Plugins.SimpleTest
{
    public sealed class SimpleTestPlugin : IPlugin
    {
        const string ChatChannelName = "slacking";

        const ushort ChatChannelPassword = 4321;

        const uint DanceId = 21;

        const uint VisibilityRange = 100;

        public string Name => "simple-test";

        static readonly Log _log = new Log(typeof(SimpleTestPlugin));

        readonly PluginContext _context;

#pragma warning disable IDE0051 // Remove unused private members
        SimpleTestPlugin(PluginContext context)
#pragma warning restore IDE0051 // Remove unused private members
        {
            _context = context;
        }

        public void Start()
        {
            var dispatch = _context.Dispatch;

            dispatch.AddHandler("S_SPAWN_ME", (client, direction, packet, flags) =>
            {
                // Automatically join a chat channel when logging in.

                _log.Info("Making client {0} join chat channel {1} with password {2}",
                    client.EndPoint, ChatChannelName, ChatChannelPassword);

                using var writer = new GameBinaryWriter();

                writer.WriteOffset(sizeof(ushort) * 2);
                writer.WriteUInt16(ChatChannelPassword);
                writer.WriteString(ChatChannelName);

                client.SendToServer(new RawPacket("C_JOIN_PRIVATE_CHANNEL")
                {
                    Payload = writer.ToArray(),
                });

                // Let's request an absurdly low visibility range.

                _log.Info("Setting visibility range for client {0} to {1}", client.EndPoint,
                    VisibilityRange);

                client.SendToServer(new CSetVisibleRangePacket
                {
                    Range = VisibilityRange,
                });

                return true;
            });

            dispatch.AddHandler("S_INVEN", (client, direction, packet, flags) =>
            {
                // Deny opening the inventory for the client.

                _log.Info("Denying inventory list for client {0}", client.EndPoint);

                return false;
            });

            dispatch.AddHandler<CCheckVersionPacket>((client, direction, packet, flags) =>
            {
                foreach (var ver in packet.Versions)
                    _log.Info("Client reported version: {0}", ver.Value);

                return true;
            });

            dispatch.AddHandler<CSocialPacket>((client, direction, packet, flags) =>
            {
                // Only allow the dance emote.

                _log.Info("Client {0} requested emote {1}; sending {2}",
                    client.EndPoint, packet.SocialId, DanceId);

                packet.SocialId = DanceId;

                return true;
            });

            _log.Basic("Simple test plugin started");
        }

        public void Stop()
        {
            _log.Basic("Simple test plugin stopped");
        }
    }
}
