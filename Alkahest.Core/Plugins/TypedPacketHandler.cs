using Alkahest.Core.Net.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Plugins
{
    public delegate bool TypedPacketHandler<T>(GameClient client, Direction direction, T packet,
        PacketFlags flags)
        where T : SerializablePacket;
}
