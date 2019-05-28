namespace Alkahest.Core.Net.Game
{
    public delegate bool PacketHandler<T>(GameClient client, Direction direction, T packet)
        where T : Packet;
}
