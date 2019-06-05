using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;
using System.Collections.Generic;
using System.Numerics;

namespace Alkahest.Core.Net.Game.Packets
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
            public sealed class CustomStringInfo
            {
                [PacketField]
                public uint Index { get; set; }

                [PacketField]
                public string CustomString { get; set; }
            }

            [PacketField]
            public List<CustomStringInfo> CustomStrings { get; } = new List<CustomStringInfo>();

            [PacketField]
            public string UserName { get; set; }

            [PacketField]
            public List<byte> Details { get; } = new List<byte>();

            [PacketField]
            public List<byte> Shape { get; } = new List<byte>();

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
            public ulong MaxHP { get; set; }

            [PacketField]
            public uint MaxMP { get; set; }

            [PacketField]
            public uint WorldId { get; set; }

            [PacketField]
            public uint GuardId { get; set; }

            [PacketField]
            public uint SectionId { get; set; }

            [PacketField]
            public ulong LastOnlineTime { get; set; }

            [PacketField]
            public bool IsDeleting { get; set; }

            [PacketField]
            public long Unknown2 { get; set; }

            [PacketField]
            public int Unknown3 { get; set; }

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
            public int Unknown4 { get; set; }

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
            public Appearance Appearance { get; set; }

            [PacketField]
            public bool IsSecondCharacter { get; set; }

            [PacketField]
            public uint AdministratorLevel { get; set; }

            [PacketField]
            public bool IsBanned { get; set; }

            [PacketField]
            public long Unknown5 { get; set; }

            [PacketField]
            public int Unknown6 { get; set; }

            [PacketField]
            public int Unknown7 { get; set; }

            [PacketField]
            public int Unknown8 { get; set; }

            [PacketField]
            public int Unknown9 { get; set; }

            [PacketField]
            public int Unknown10 { get; set; }

            [PacketField]
            public int Unknown11 { get; set; }

            [PacketField]
            public int Unknown12 { get; set; }

            [PacketField]
            public int Unknown13 { get; set; }

            [PacketField]
            public int Unknown14 { get; set; }

            [PacketField]
            public int Unknown15 { get; set; }

            [PacketField]
            public int Unknown16 { get; set; }

            [PacketField]
            public int Unknown17 { get; set; }

            [PacketField]
            public int Unknown18 { get; set; }

            [PacketField]
            public int Unknown19 { get; set; }

            [PacketField]
            public int Unknown20 { get; set; }

            [PacketField]
            public int Unknown21 { get; set; }

            [PacketField]
            public int Unknown22 { get; set; }

            [PacketField]
            public int Unknown23 { get; set; }

            [PacketField]
            public int Unknown24 { get; set; }

            [PacketField]
            public int Unknown25 { get; set; }

            [PacketField]
            public int Unknown26 { get; set; }

            [PacketField]
            public int Unknown27 { get; set; }

            [PacketField]
            public int Unknown28 { get; set; }

            [PacketField]
            public int Unknown29 { get; set; }

            [PacketField]
            public int Unknown30 { get; set; }

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
            public uint FootprintItemId { get; set; }

            [PacketField]
            public int Unknown31 { get; set; }

            [PacketField]
            public uint WeaponEnchantmentLevel { get; set; }

            [PacketField]
            public uint CurrentRestedExperience { get; set; }

            [PacketField]
            public uint MaxRestedExperience { get; set; }

            [PacketField]
            public byte Unknown32 { get; set; }

            [PacketField]
            public float HairCostumeScale { get; set; }

            [PacketField]
            public Vector3 HairCostumeRotation { get; set; }

            [PacketField]
            public Vector3 HairCostumeTranslation { get; set; }

            [PacketField]
            public Vector3 HairCostumeTranslationDebug { get; set; }

            [PacketField]
            public float FaceCostumeScale { get; set; }

            [PacketField]
            public Vector3 FaceCostumeRotation { get; set; }

            [PacketField]
            public Vector3 FaceCostumeTranslation { get; set; }

            [PacketField]
            public Vector3 FaceCostumeTranslationDebug { get; set; }

            [PacketField]
            public float BackCostumeScale { get; set; }

            [PacketField]
            public Vector3 BackCostumeRotation { get; set; }

            [PacketField]
            public Vector3 BackCostumeTranslation { get; set; }

            [PacketField]
            public Vector3 BackCostumeTranslationDebug { get; set; }

            [PacketField]
            public byte Unknown33 { get; set; }

            [PacketField]
            public bool IsNewCharacter { get; set; }

            [PacketField]
            public int Unknown34 { get; set; }

            [PacketField]
            public byte Unknown35 { get; set; }

            [PacketField]
            public int Unknown36 { get; set; }

            [PacketField]
            public int Unknown37 { get; set; }

            [PacketField]
            public LaurelKind Laurel { get; set; }

            [PacketField]
            public uint Order { get; set; }

            [PacketField]
            public uint GuildLogoId { get; set; }

            [PacketField]
            public uint ApexLevel { get; set; }
        }

        [PacketField]
        public List<CharacterInfo> Characters { get; } = new List<CharacterInfo>();

        [PacketField]
        public bool IsVeteran { get; set; }

        [PacketField]
        public int Unknown38 { get; set; }

        [PacketField]
        public uint MaxCharacters { get; set; }

        [PacketField]
        public bool IsFirst { get; set; }

        [PacketField]
        public bool IsIncomplete { get; set; }

        [PacketField]
        public int Unknown39 { get; set; }

        [PacketField]
        public int Unknown40 { get; set; }

        [PacketField]
        public int Unknown41 { get; set; }

        [PacketField]
        public int Unknown42 { get; set; }
    }
}
