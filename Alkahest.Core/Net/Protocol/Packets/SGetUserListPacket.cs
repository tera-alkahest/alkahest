using System.Collections.Generic;
using Alkahest.Core.Game;

namespace Alkahest.Core.Net.Protocol.Packets
{
    public sealed class SGetUserListPacket : Packet
    {
        const string Name = "S_GET_USER_LIST";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SGetUserListPacket();
        }

        public sealed class CharacterInfo
        {
            [PacketField(IsUnknownArray = true)]
            public ushort Unknown1Count { get; set; }

            [PacketField(IsUnknownArray = true)]
            public ushort Unknown1Offset { get; set; }

            [PacketField]
            public string UserName { get; set; }

            [PacketField]
            public List<byte> Details1 { get; } = new List<byte>();

            [PacketField]
            public List<byte> Details2 { get; } = new List<byte>();

            [PacketField]
            public string GuildName { get; set; }

            [PacketField]
            public uint PlayerId { get; set; }

            [PacketField]
            public Gender Gender { get; set; }

            [PacketField]
            public Race Race { get; set; }

            [PacketField]
            public Class Class { get; set; }

            [PacketField]
            public uint Level { get; set; }

            [PacketField]
            public uint Unknown2 { get; set; }

            [PacketField]
            public uint Unknown3 { get; set; }

            [PacketField]
            public uint WorldMapWorldId { get; set; }

            [PacketField]
            public uint WorldMapGuardId { get; set; }

            [PacketField]
            public uint AreaNameId { get; set; }

            [PacketField]
            public ulong LastOnlineTime { get; set; }

            [PacketField]
            public byte Unknown4 { get; set; }

            [PacketField]
            public uint Unknown5 { get; set; }

            [PacketField]
            public uint Unknown6 { get; set; }

            [PacketField]
            public uint Unknown7 { get; set; }

            [PacketField]
            public uint WeaponItemId { get; set; }

            [PacketField]
            public uint Earring1ItemId { get; set; }

            [PacketField]
            public uint Earring2ItemId { get; set; }

            [PacketField]
            public uint ArmorItemId { get; set; }

            [PacketField]
            public uint HandwearItemId { get; set; }

            [PacketField]
            public uint FootwearItemId { get; set; }

            [PacketField]
            public uint Unknown8 { get; set; }

            [PacketField]
            public uint Ring1ItemId { get; set; }

            [PacketField]
            public uint Ring2ItemId { get; set; }

            [PacketField]
            public uint UnderwearItemId { get; set; }

            [PacketField]
            public uint CircletItemId { get; set; }

            [PacketField]
            public uint MaskItemId { get; set; }

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
            public uint Unknown9 { get; set; }

            [PacketField]
            public uint Unknown10 { get; set; }

            [PacketField]
            public uint Unknown11 { get; set; }

            [PacketField]
            public ushort Unknown12 { get; set; }

            [PacketField]
            public uint Unknown13 { get; set; }

            [PacketField]
            public uint Unknown14 { get; set; }

            [PacketField]
            public uint Unknown15 { get; set; }

            [PacketField]
            public uint Unknown16 { get; set; }

            [PacketField]
            public uint Unknown17 { get; set; }

            [PacketField]
            public uint Unknown18 { get; set; }

            [PacketField]
            public uint Unknown19 { get; set; }

            [PacketField]
            public uint Unknown20 { get; set; }

            [PacketField]
            public uint Unknown21 { get; set; }

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
            public uint Unknown34 { get; set; }

            [PacketField]
            public uint Unknown35 { get; set; }

            [PacketField]
            public uint Unknown36 { get; set; }

            [PacketField]
            public uint Unknown37 { get; set; }

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
            public uint Unknown38 { get; set; }

            [PacketField]
            public uint Unknown39 { get; set; }

            [PacketField]
            public uint CurrentRestedExperience { get; set; }

            [PacketField]
            public uint MaxRestedExperience { get; set; }

            [PacketField]
            public ushort Unknown40 { get; set; }

            [PacketField]
            public uint Unknown41 { get; set; }

            [PacketField]
            public byte Unknown42 { get; set; }

            [PacketField]
            public uint Unknown43 { get; set; }

            [PacketField]
            public uint Unknown44 { get; set; }

            [PacketField]
            public LaurelKind Laurel { get; set; }

            [PacketField]
            public uint Unknown45 { get; set; }

            [PacketField]
            public uint Unknown46 { get; set; }
        }

        [PacketField]
        public List<CharacterInfo> Characters { get; } =
            new List<CharacterInfo>();

        [PacketField]
        public byte Unknown47 { get; set; }

        [PacketField]
        public uint Unknown48 { get; set; }

        [PacketField]
        public uint MaxCharacters { get; set; }

        [PacketField]
        public ushort Unknown49 { get; set; }

        [PacketField]
        public uint Unknown50 { get; set; }

        [PacketField]
        public uint Unknown51 { get; set; }

        [PacketField]
        public uint Unknown52 { get; set; }

        [PacketField]
        public uint Unknown53 { get; set; }
    }
}
