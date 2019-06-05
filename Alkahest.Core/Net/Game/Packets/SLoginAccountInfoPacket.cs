using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_LOGIN_ACCOUNT_INFO")]
    public sealed class SLoginAccountInfoPacket : SerializablePacket
    {
        public string ServerName { get; set; }

        public ulong AccountId { get; set; }
    }
}
