using Alkahest.Core.Collections;
using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;
using System.Collections.Generic;
using System.Numerics;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_GET_USER_LIST")]
    public sealed class SGetUserListPacket : SerializablePacket
    {
        public sealed class CharacterInfo
        {
            public sealed class CustomStringInfo
            {
                public uint Index { get; set; }

                public string CustomString { get; set; }
            }

            public NoNullList<CustomStringInfo> CustomStrings { get; } =
                new NoNullList<CustomStringInfo>();

            public string UserName { get; set; }

            public List<byte> Details { get; } = new List<byte>();

            public List<byte> Shape { get; } = new List<byte>();

            public string GuildName { get; set; }

            public uint PlayerId { get; set; }

            public Gender Gender { get; set; }

            public Race Race { get; set; }

            public Class Class { get; set; }

            public uint Level { get; set; }

            public ulong MaxHP { get; set; }

            public uint MaxMP { get; set; }

            public uint WorldId { get; set; }

            public uint GuardId { get; set; }

            public uint SectionId { get; set; }

            public ulong LastOnlineTime { get; set; }

            public bool IsDeleting { get; set; }

            public long Unknown2 { get; set; }

            public int Unknown3 { get; set; }

            public uint WeaponItemId { get; set; }

            public uint Earring1ItemId { get; set; }

            public uint Earring2ItemId { get; set; }

            public uint ArmorItemId { get; set; }

            public uint HandwearItemId { get; set; }

            public uint FootwearItemId { get; set; }

            public int Unknown4 { get; set; }

            public uint Ring1ItemId { get; set; }

            public uint Ring2ItemId { get; set; }

            public uint UnderwearItemId { get; set; }

            public uint CircletItemId { get; set; }

            public uint MaskItemId { get; set; }

            public Appearance Appearance { get; set; }

            public bool IsSecondCharacter { get; set; }

            public uint AdministratorLevel { get; set; }

            public bool IsBanned { get; set; }

            public long Unknown5 { get; set; }

            public int Unknown6 { get; set; }

            public int Unknown7 { get; set; }

            public int Unknown8 { get; set; }

            public int Unknown9 { get; set; }

            public int Unknown10 { get; set; }

            public int Unknown11 { get; set; }

            public int Unknown12 { get; set; }

            public int Unknown13 { get; set; }

            public int Unknown14 { get; set; }

            public int Unknown15 { get; set; }

            public int Unknown16 { get; set; }

            public int Unknown17 { get; set; }

            public int Unknown18 { get; set; }

            public int Unknown19 { get; set; }

            public int Unknown20 { get; set; }

            public int Unknown21 { get; set; }

            public int Unknown22 { get; set; }

            public int Unknown23 { get; set; }

            public int Unknown24 { get; set; }

            public int Unknown25 { get; set; }

            public int Unknown26 { get; set; }

            public int Unknown27 { get; set; }

            public int Unknown28 { get; set; }

            public int Unknown29 { get; set; }

            public int Unknown30 { get; set; }

            public uint HairCostumeItemId { get; set; }

            public uint FaceCostumeItemId { get; set; }

            public uint BackCostumeItemId { get; set; }

            public uint WeaponSkinItemId { get; set; }

            public uint BodyCostumeItemId { get; set; }

            public uint FootprintItemId { get; set; }

            public int Unknown31 { get; set; }

            public uint WeaponEnchantmentLevel { get; set; }

            public uint CurrentRestedExperience { get; set; }

            public uint MaxRestedExperience { get; set; }

            public byte Unknown32 { get; set; }

            public float HairCostumeScale { get; set; }

            public Vector3 HairCostumeRotation { get; set; }

            public Vector3 HairCostumeTranslation { get; set; }

            public Vector3 HairCostumeTranslationDebug { get; set; }

            public float FaceCostumeScale { get; set; }

            public Vector3 FaceCostumeRotation { get; set; }

            public Vector3 FaceCostumeTranslation { get; set; }

            public Vector3 FaceCostumeTranslationDebug { get; set; }

            public float BackCostumeScale { get; set; }

            public Vector3 BackCostumeRotation { get; set; }

            public Vector3 BackCostumeTranslation { get; set; }

            public Vector3 BackCostumeTranslationDebug { get; set; }

            public byte Unknown33 { get; set; }

            public bool IsNewCharacter { get; set; }

            public int Unknown34 { get; set; }

            public byte Unknown35 { get; set; }

            public int Unknown36 { get; set; }

            public int Unknown37 { get; set; }

            public LaurelKind Laurel { get; set; }

            public uint Order { get; set; }

            public uint GuildLogoId { get; set; }

            public uint ApexLevel { get; set; }
        }

        public NoNullList<CharacterInfo> Characters { get; } = new NoNullList<CharacterInfo>();

        public bool IsVeteran { get; set; }

        public int Unknown38 { get; set; }

        public uint MaxCharacters { get; set; }

        public bool IsFirst { get; set; }

        public bool IsIncomplete { get; set; }

        public int Unknown39 { get; set; }

        public int Unknown40 { get; set; }

        public int Unknown41 { get; set; }

        public int Unknown42 { get; set; }
    }
}
