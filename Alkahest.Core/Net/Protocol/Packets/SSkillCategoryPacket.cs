namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class SSkillCategoryPacket : Packet
    {
        const string Name = "S_SKILL_CATEGORY";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SSkillCategoryPacket();
        }

        [PacketField]
        public uint Category { get; set; }

        [PacketField]
        public byte Unknown2 { get; set; }
    }
}
