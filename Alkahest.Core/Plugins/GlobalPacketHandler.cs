using Alkahest.Core.Net.Game;

namespace Alkahest.Core.Plugins
{
    public delegate bool GlobalPacketHandler(GameClient client, Direction direction, ushort code,
        RawPacket packet, PacketFlags flags);
}
