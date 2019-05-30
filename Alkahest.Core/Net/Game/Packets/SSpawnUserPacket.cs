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
        public List<byte> Details1 { get; } = new List<byte>();

        [PacketField]
        public string GuildLogoName { get; set; }

        [PacketField]
        public List<byte> Details2 { get; } = new List<byte>();

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
        public uint Relation { get; set; }

        [PacketField]
        public TemplateId Template { get; set; }

        [PacketField]
        public ushort Unknown1 { get; set; }

        [PacketField]
        public ushort Unknown2 { get; set; }

        [PacketField]
        public ushort Unknown3 { get; set; }

        [PacketField]
        public ushort Unknown4 { get; set; }

        [PacketField]
        public ushort Unknown5 { get; set; }

        [PacketField]
        public byte Unknown6 { get; set; }

        [PacketField]
        public byte Unknown7 { get; set; }

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
        public uint Unknown8 { get; set; }

        [PacketField]
        public uint Unknown9 { get; set; }

        [PacketField]
        public uint Unknown10 { get; set; }

        [PacketField]
        public uint Unknown11 { get; set; }

        [PacketField]
        public uint Unknown12 { get; set; }

        [PacketField]
        public uint Unknown13 { get; set; }

        [PacketField]
        public byte Unknown14 { get; set; }

        [PacketField]
        public ushort Unknown15 { get; set; }

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
        public uint WeaponEnchantmentLevel { get; set; }

        [PacketField]
        public ushort Unknown29 { get; set; }

        [PacketField]
        public uint Level { get; set; }

        [PacketField]
        public uint Unknown30 { get; set; }

        [PacketField]
        public uint Unknown31 { get; set; }

        [PacketField]
        public byte Unknown32 { get; set; }

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
        public uint Unknown33 { get; set; }

        [PacketField]
        public uint Unknown34 { get; set; }

        [PacketField]
        public uint Unknown35 { get; set; }

        [PacketField]
        public byte Unknown36 { get; set; }

        [PacketField]
        public byte Unknown37 { get; set; }

        [PacketField]
        public uint Unknown38 { get; set; }

        [PacketField]
        public uint Unknown39 { get; set; }

        [PacketField]
        public uint Unknown40 { get; set; }

        [PacketField]
        public float Unknown41 { get; set; }
    }
}
