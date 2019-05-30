using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class CCompleteDailyEventPacket : Packet
    {
        const string Name = "C_COMPLETE_DAILY_EVENT";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new CCompleteDailyEventPacket();
        }

        [PacketField]
        public uint DailyQuestId { get; set; }
    }
}
