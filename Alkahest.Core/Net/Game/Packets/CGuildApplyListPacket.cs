namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class CGuildApplyListPacket : Packet
    {
        const string Name = "C_GUILD_APPLY_LIST";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CGuildApplyListPacket();
        }
    }
}
