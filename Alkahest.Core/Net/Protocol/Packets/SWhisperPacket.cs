using Alkahest.Core.Data;

namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class SWhisperPacket : Packet
    {
        const string Name = "S_WHISPER";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SWhisperPacket();
        }

        [PacketField]
        public string SenderName { get; set; }

        [PacketField]
        public string RecipientName { get; set; }

        [PacketField]
        public string Message { get; set; }

        [PacketField]
        public EntityId Source { get; set; }

        [PacketField]
        public byte Unknown1 { get; set; }

        [PacketField]
        public bool IsGameMaster { get; set; }

        [PacketField]
        public byte Unknown2 { get; set; }
    }
}
