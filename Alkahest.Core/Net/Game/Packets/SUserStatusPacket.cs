using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_USER_STATUS")]
    public sealed class SUserStatusPacket : SerializablePacket
    {
        public GameId Target { get; set; }

        public UserStatus Status { get; set; }

        public short Unknown1 { get; set; }

        public byte Unknown2 { get; set; }
    }
}
