namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class CUpdateFriendInfoPacket : Packet
    {
        const string Name = "C_UPDATE_FRIEND_INFO";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CUpdateFriendInfoPacket();
        }
    }
}
