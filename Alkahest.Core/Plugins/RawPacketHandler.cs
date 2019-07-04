using Alkahest.Core.Net.Game;

namespace Alkahest.Core.Plugins
{
    public delegate bool RawPacketHandler(GameClient client, Direction direction, RawPacket packet,
        PacketFlags flags);
}
