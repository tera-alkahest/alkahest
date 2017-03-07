namespace Alkahest.Core.Net.Protocol
{
    public delegate bool PacketHandler<T>(GameClient client,
        Direction direction, T packet)
        where T : Packet;
}
