namespace Alkahest.Core.Net.Game
{
    public delegate bool RawPacketHandler(GameClient client, Direction direction, RawPacket packet);
}
