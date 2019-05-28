namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SUpdateEventSeedStatePacket : Packet
    {
        const string Name = "S_UPDATE_EVENT_SEED_STATE";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SUpdateEventSeedStatePacket();
        }

        [PacketField]
        public ulong Unknown1 { get; set; }

        [PacketField]
        public uint Unknown2 { get; set; }
    }
}
