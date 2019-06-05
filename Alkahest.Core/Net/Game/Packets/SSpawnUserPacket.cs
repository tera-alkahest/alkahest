using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;
using System.Collections.Generic;
using System.Numerics;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SSpawnUserPacket : Packet
    {
        const string Name = "S_SPAWN_USER";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SSpawnUserPacket();
        }

        public sealed class IconInfo
        {
            [PacketField]
            public uint IconId { get; set; }
        }

        [PacketField]
        public List<IconInfo> Icons { get; } = new List<IconInfo>();

        public sealed class PackageInfo
        {
            [PacketField]
            public uint PackageId { get; set; }
        }

        [PacketField]
        public List<PackageInfo> Packages { get; } = new List<PackageInfo>();

        [PacketField]
        public string UserName { get; set; }

        [PacketField]
        public string GuildName { get; set; }

        [PacketField]
        public string GuildRankName { get; set; }

        [PacketField]
        public List<byte> Details { get; } = new List<byte>();

        [PacketField]
        public string GuildLogoName { get; set; }

        [PacketField]
        public List<byte> Shape { get; } = new List<byte>();

        [PacketField]
        public uint ServerId { get; set; }

        [PacketField]
        public uint PlayerId { get; set; }

        [PacketField]
        public GameId Target { get; set; }

        [PacketField]
        public Vector3 Position { get; set; }

        [PacketField]
        public Angle Direction { get; set; }

        [PacketField]
        public int Unknown1 { get; set; }

        [PacketField]
        public TemplateId Template { get; set; }

        [PacketField]
        public ushort HuntingZoneId { get; set; }

        [PacketField]
        public ushort WalkSpeed { get; set; }

        [PacketField]
        public ushort RunSpeed { get; set; }

        [PacketField]
        public short Unknown2 { get; set; }

        [PacketField]
        public short Unknown3 { get; set; }

        [PacketField]
        public bool IsVisible { get; set; }

        [PacketField]
        public bool IsAlive { get; set; }

        [PacketField]
        public Appearance Appearance { get; set; }

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
        public int Unknown4 { get; set; }

        [PacketField]
        public int Unknown5 { get; set; }

        [PacketField]
        public int Unknown6 { get; set; }

        [PacketField]
        public uint TitleAchievementId { get; set; }

        [PacketField]
        public long Unknown7 { get; set; }

        [PacketField]
        public uint GuildLogoId { get; set; }

        [PacketField]
        public byte Unknown8 { get; set; }

        [PacketField]
        public bool IsGameMaster { get; set; }

        [PacketField]
        public bool IsGameMasterInvisible { get; set; }

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
        public uint WeaponEnchantmentLevel { get; set; }

        [PacketField]
        public bool IsWorldEventTarget { get; set; }

        [PacketField]
        public byte Unknown21 { get; set; }

        [PacketField]
        public uint Level { get; set; }

        [PacketField]
        public long Unknown22 { get; set; }

        [PacketField]
        public byte Unknown23 { get; set; }

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
        public byte Unknown24 { get; set; }

        [PacketField]
        public int Unknown25 { get; set; }

        [PacketField]
        public int Unknown26 { get; set; }

        [PacketField]
        public int Unknown27 { get; set; }

        [PacketField]
        public byte Unknown28 { get; set; }

        [PacketField]
        public byte Unknown29 { get; set; }

        [PacketField]
        public long Unknown30 { get; set; }

        [PacketField]
        public int Unknown31 { get; set; }

        [PacketField]
        public float Scale { get; set; }
    }
}
