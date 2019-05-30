using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class CGetUserListPacket : Packet
    {
        const string Name = "C_GET_USER_LIST";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CGetUserListPacket();
        }
    }
}
