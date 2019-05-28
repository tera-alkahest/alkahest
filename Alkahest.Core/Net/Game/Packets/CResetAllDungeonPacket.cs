namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class CResetAllDungeonPacket : Packet
    {
        const string Name = "C_RESET_ALL_DUNGEON";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CResetAllDungeonPacket();
        }
    }
}
