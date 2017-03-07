namespace Alkahest.Core.Net.Protocol
{
    public delegate bool RawPacketHandler(GameClient client,
        Direction direction, RawPacket packet);
}
