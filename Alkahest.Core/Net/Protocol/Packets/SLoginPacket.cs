using System.Collections.Generic;
using Alkahest.Core.Data;

namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class SLoginPacket : Packet
    {
        const string Name = "S_LOGIN";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SLoginPacket();
        }

        [PacketField]
        public string UserName { get; set; }

        [PacketField]
        public List<byte> Details1 { get; } = new List<byte>();

        [PacketField]
        public List<byte> Details2 { get; } = new List<byte>();

        [PacketField]
        public TemplateId Template { get; set; }

        [PacketField]
        public EntityId Target { get; set; }

        [PacketField]
        public uint ServerId { get; set; }

        [PacketField]
        public uint PlayerId { get; set; }

        [PacketField]
        public uint Unknown1 { get; set; }

        [PacketField]
        public byte Unknown2 { get; set; }

        [PacketField]
        public uint Unknown3 { get; set; }

        [PacketField]
        public uint Unknown4 { get; set; }

        [PacketField]
        public uint Unknown5 { get; set; }

        [PacketField]
        public byte Appearance1 { get; set; }

        [PacketField]
        public byte Appearance2 { get; set; }

        [PacketField]
        public byte Appearance3 { get; set; }

        [PacketField]
        public byte Appearance4 { get; set; }

        [PacketField]
        public byte Appearance5 { get; set; }

        [PacketField]
        public byte Appearance6 { get; set; }

        [PacketField]
        public byte Appearance7 { get; set; }

        [PacketField]
        public byte Appearance8 { get; set; }

        [PacketField]
        public ushort Unknown6 { get; set; }

        [PacketField]
        public ushort Level { get; set; }

        [PacketField]
        public ushort EnergyGatheringLevel { get; set; }

        [PacketField]
        public ushort Unknown7 { get; set; }

        [PacketField]
        public ushort PlantGatheringLevel { get; set; }

        [PacketField]
        public ushort OreGatheringLevel { get; set; }

        [PacketField]
        public uint Unknown8 { get; set; }

        [PacketField]
        public uint Unknown9 { get; set; }

        [PacketField]
        public ushort Unknown10 { get; set; }

        [PacketField]
        public ulong TotalExperience { get; set; }

        [PacketField]
        public ulong ShownExperience { get; set; }

        [PacketField]
        public ulong NeededExperience { get; set; }

        [PacketField]
        public uint Unknown11 { get; set; }

        [PacketField]
        public uint Unknown12 { get; set; }

        [PacketField]
        public uint Unknown13 { get; set; }

        [PacketField]
        public uint Unknown14 { get; set; }

        [PacketField]
        public uint CurrentRestedExperience { get; set; }

        [PacketField]
        public uint MaxRestedExperience { get; set; }

        [PacketField]
        public float Unknown15 { get; set; }

        [PacketField]
        public uint Unknown16 { get; set; }

        [PacketField]
        public uint WeaponItemId { get; set; }

        [PacketField]
        public uint ArmorItemId { get; set; }

        [PacketField]
        public uint HandwearItemId { get; set; }

        [PacketField]
        public uint FootwearItemId { get; set; }

        [PacketField]
        public uint UnderwearItemId { get; set; }

        [PacketField]
        public uint CircletItemId { get; set; }

        [PacketField]
        public uint MaskItemId { get; set; }

        [PacketField]
        public uint Unknown17 { get; set; }

        [PacketField]
        public uint Unknown18 { get; set; }

        [PacketField]
        public byte Unknown19 { get; set; }

        [PacketField]
        public uint Unknown20 { get; set; }

        [PacketField]
        public uint Unknown21 { get; set; }

        [PacketField]
        public uint TitleAchievementId { get; set; }

        [PacketField]
        public uint Unknown22 { get; set; }

        [PacketField]
        public uint Unknown23 { get; set; }

        [PacketField]
        public uint Unknown24 { get; set; }

        [PacketField]
        public uint Unknown25 { get; set; }

        [PacketField]
        public uint Unknown26 { get; set; }

        [PacketField]
        public uint Unknown27 { get; set; }

        [PacketField]
        public uint Unknown28 { get; set; }

        [PacketField]
        public uint Unknown29 { get; set; }

        [PacketField]
        public uint Unknown30 { get; set; }

        [PacketField]
        public uint Unknown31 { get; set; }

        [PacketField]
        public uint Unknown32 { get; set; }

        [PacketField]
        public uint Unknown33 { get; set; }

        [PacketField]
        public uint WeaponEnchantmentLevel { get; set; }

        [PacketField]
        public uint Unknown34 { get; set; }

        [PacketField]
        public byte Unknown35 { get; set; }

        [PacketField]
        public byte Unknown36 { get; set; }

        [PacketField]
        public uint HairCostumeItemId { get; set; }

        [PacketField]
        public uint FaceCostumeItemId { get; set; }

        [PacketField]
        public uint BackCostumeItemId { get; set; }

        [PacketField]
        public uint WeaponSkinItemId { get; set; }

        [PacketField]
        public uint BodyCostumeItemId { get; set; }

        [PacketField]
        public uint Unknown37 { get; set; }

        [PacketField]
        public byte Unknown38 { get; set; }

        [PacketField]
        public ushort Unknown39 { get; set; }

        [PacketField]
        public uint Unknown40 { get; set; }

        [PacketField]
        public ushort Unknown41 { get; set; }

        [PacketField]
        public ulong Unknown42 { get; set; }

        [PacketField]
        public uint Unknown43 { get; set; }

        [PacketField]
        public byte Unknown44 { get; set; }
    }
}
