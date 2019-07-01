using Alkahest.Core.Collections;
using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;
using System.Collections.Generic;
using System.Numerics;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_SPAWN_USER")]
    public sealed class SSpawnUserPacket : SerializablePacket
    {
        public sealed class IconInfo
        {
            public uint IconId { get; set; }
        }

        public NoNullList<IconInfo> Icons { get; } = new NoNullList<IconInfo>();

        public sealed class PackageInfo
        {
            public uint PackageId { get; set; }
        }

        public NoNullList<PackageInfo> Packages { get; } = new NoNullList<PackageInfo>();

        public string UserName { get; set; }

        public string GuildName { get; set; }

        public string GuildRankName { get; set; }

        public List<byte> Details { get; } = new List<byte>();

        public string GuildLogoName { get; set; }

        public List<byte> Shape { get; } = new List<byte>();

        public uint ServerId { get; set; }

        public uint PlayerId { get; set; }

        public GameId Target { get; set; }

        public Vector3 Position { get; set; }

        public Angle Direction { get; set; }

        public int Unknown1 { get; set; }

        public TemplateId Template { get; set; }

        public ushort HuntingZoneId { get; set; }

        public ushort WalkSpeed { get; set; }

        public ushort RunSpeed { get; set; }

        public short Unknown2 { get; set; }

        public short Unknown3 { get; set; }

        public bool IsVisible { get; set; }

        public bool IsAlive { get; set; }

        public Appearance Appearance { get; set; }

        public uint WeaponItemId { get; set; }

        public uint ArmorItemId { get; set; }

        public uint HandwearItemId { get; set; }

        public uint FootwearItemId { get; set; }

        public uint UnderwearItemId { get; set; }

        public uint CircletItemId { get; set; }

        public uint MaskItemId { get; set; }

        public int Unknown4 { get; set; }

        public int Unknown5 { get; set; }

        public int Unknown6 { get; set; }

        public uint TitleAchievementId { get; set; }

        public long Unknown7 { get; set; }

        public uint GuildLogoId { get; set; }

        public byte Unknown8 { get; set; }

        public bool IsGameMaster { get; set; }

        public bool IsGameMasterInvisible { get; set; }

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

        public uint WeaponEnchantmentLevel { get; set; }

        public bool IsWorldEventTarget { get; set; }

        public byte Unknown21 { get; set; }

        public uint Level { get; set; }

        public long Unknown22 { get; set; }

        public byte Unknown23 { get; set; }

        public uint HairCostumeItemId { get; set; }

        public uint FaceCostumeItemId { get; set; }

        public uint BackCostumeItemId { get; set; }

        public uint WeaponSkinItemId { get; set; }

        public uint BodyCostumeItemId { get; set; }

        public uint FootprintItemId { get; set; }

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

        public byte Unknown24 { get; set; }

        public int Unknown25 { get; set; }

        public int Unknown26 { get; set; }

        public int Unknown27 { get; set; }

        public byte Unknown28 { get; set; }

        public byte Unknown29 { get; set; }

        public long Unknown30 { get; set; }

        public int Unknown31 { get; set; }

        public float Scale { get; set; }
    }
}
