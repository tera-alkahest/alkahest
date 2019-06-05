using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;
using System.Collections.Generic;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("C_LOGIN_ARBITER")]
    public sealed class CLoginArbiterPacket : SerializablePacket
    {
        public string AccountName { get; set; }

        public List<byte> Ticket { get; } = new List<byte>();

        public int Unknown1 { get; set; }

        public byte Unknown2 { get; set; }

        public ClientRegion Region { get; set; }

        public uint PatchVersion { get; set; }
    }
}
