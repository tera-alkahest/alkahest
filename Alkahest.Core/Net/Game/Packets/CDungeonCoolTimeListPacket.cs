namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class CDungeonCoolTimeListPacket : Packet
    {
        const string Name = "C_DUNGEON_COOL_TIME_LIST";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CDungeonCoolTimeListPacket();
        }
    }
}
