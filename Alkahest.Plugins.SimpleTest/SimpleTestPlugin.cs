using Alkahest.Core;
using Alkahest.Core.IO;
using Alkahest.Core.Logging;
using Alkahest.Core.Net.Game;
using Alkahest.Core.Net.Game.Packets;
using Alkahest.Core.Plugins;
using System.Collections.Generic;
using System.Linq;

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

        readonly List<(string, RawPacketHandler)> _rawHandlers;

#pragma warning disable IDE0051 // Remove unused private members
        SimpleTestPlugin(PluginContext context)
#pragma warning restore IDE0051 // Remove unused private members
        {
            _context = context;
            _rawHandlers = new List<(string, RawPacketHandler)>
            {
                ("S_SPAWN_ME", HandleSpawnMe),
                ("S_INVEN", HandleInventory),
            };
        }

        public void Start()
        {
            foreach (var proc in _context.Proxies.Select(x => x.Processor))
            {
                foreach (var (name, handler) in _rawHandlers)
                    proc.AddRawHandler(name, handler);

                proc.AddHandler<CCheckVersionPacket>(HandleCheckVersion);
                proc.AddHandler<CSocialPacket>(HandleSocial);
            }

            _log.Basic("Simple test plugin started");
        }

        public void Stop()
        {
            foreach (var proc in _context.Proxies.Select(x => x.Processor))
            {
                foreach (var (name, handler) in _rawHandlers)
                    proc.RemoveRawHandler(name, handler);

                proc.RemoveHandler<CCheckVersionPacket>(HandleCheckVersion);
                proc.RemoveHandler<CSocialPacket>(HandleSocial);
            }

            _log.Basic("Simple test plugin stopped");
        }

        bool HandleCheckVersion(GameClient client, Direction direction, CCheckVersionPacket packet)
        {
            foreach (var ver in packet.Versions)
                _log.Info("Client reported version: {0}", ver.Value);

            return true;
        }

        bool HandleSocial(GameClient client, Direction direction, CSocialPacket packet)
        {
            // Only allow the dance emote.

            _log.Info("Client {0} requested emote {1}; sending {2}",
                client.EndPoint, packet.SocialId, DanceId);

            packet.SocialId = DanceId;

            return true;
        }

        bool HandleSpawnMe(GameClient client, Direction direction, RawPacket packet)
        {
            // Automatically join a chat channel when logging in.

            _log.Info("Making client {0} join chat channel {1} with password {2}", client.EndPoint,
                ChatChannelName, ChatChannelPassword);

            using var writer = new GameBinaryWriter();

            writer.WriteOffset(sizeof(ushort) * 2);
            writer.WriteUInt16(ChatChannelPassword);
            writer.WriteString(ChatChannelName);

            client.SendToServer(new RawPacket("C_JOIN_PRIVATE_CHANNEL")
            {
                Payload = writer.ToArray(),
            });

            // Let's request an absurdly low visibility range.

            _log.Info("Setting visibility range for client {0} to {1}", client.EndPoint, VisibilityRange);

            client.SendToServer(new CSetVisibleRangePacket
            {
                Range = VisibilityRange,
            });

            return true;
        }

        bool HandleInventory(GameClient client, Direction direction, RawPacket packet)
        {
            // Deny opening the inventory for the client.

            _log.Info("Denying inventory list for client {0}", client.EndPoint);

            return false;
        }
    }
}
