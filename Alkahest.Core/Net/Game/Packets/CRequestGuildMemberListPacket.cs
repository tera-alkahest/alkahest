namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class CRequestGuildMemberListPacket : Packet
    {
        const string Name = "C_REQUEST_GUILD_MEMBER_LIST";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CRequestGuildMemberListPacket();
        }
    }
}
