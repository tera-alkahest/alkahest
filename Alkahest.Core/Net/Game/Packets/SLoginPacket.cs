using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;
using System.Collections.Generic;

namespace Alkahest.Core.Net.Game.Packets
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

        public sealed class ServantInfo
        {
            [PacketField]
            public long Unknown1 { get; set; }

            [PacketField]
            public int Unknown2 { get; set; }

            [PacketField]
            public int Unknown3 { get; set; }

            [PacketField]
            public int Unknown4 { get; set; }

            [PacketField]
            public int Unknown5 { get; set; }
        }

        [PacketField]
        public List<ServantInfo> Servants { get; } = new List<ServantInfo>();

        [PacketField]
        public string UserName { get; set; }

        [PacketField]
        public List<byte> Details { get; } = new List<byte>();

        [PacketField]
        public List<byte> Shape { get; } = new List<byte>();

        [PacketField]
        public TemplateId Template { get; set; }

        [PacketField]
        public GameId Target { get; set; }

        [PacketField]
        public uint ServerId { get; set; }

        [PacketField]
        public uint PlayerId { get; set; }

        [PacketField]
        public int Unknown6 { get; set; }

        [PacketField]
        public bool IsAlive { get; set; }

        [PacketField]
        public int Unknown7 { get; set; }

        [PacketField]
        public uint WalkSpeed { get; set; }

        [PacketField]
        public uint RunSpeed { get; set; }

        [PacketField]
        public Appearance Appearance { get; set; }

        [PacketField]
        public bool IsVisible { get; set; }

        [PacketField]
        public bool IsSecondCharacter { get; set; }

        [PacketField]
        public ushort Level { get; set; }

        [PacketField]
        public uint ApexLevel { get; set; }

        [PacketField]
        public uint OreGatheringLevel { get; set; }

        [PacketField]
        public int Unknown8 { get; set; }

        [PacketField]
        public uint PlantGatheringLevel { get; set; }

        [PacketField]
        public uint EnergyGatheringLevel { get; set; }

        [PacketField]
        public short Unknown9 { get; set; }

        [PacketField]
        public int Unknown10 { get; set; }

        [PacketField]
        public int Unknown11 { get; set; }

        [PacketField]
        public ulong TotalExperience { get; set; }

        [PacketField]
        public ulong ShownExperience { get; set; }

        [PacketField]
        public ulong NeededExperience { get; set; }

        [PacketField]
        public uint EnhancementLevel { get; set; }

        [PacketField]
        public ulong EnhancementExperience { get; set; }

        [PacketField]
        public uint EnhancementDailyExperience { get; set; }

        [PacketField]
        public uint CurrentRestedExperience { get; set; }

        [PacketField]
        public uint MaxRestedExperience { get; set; }

        [PacketField]
        public float ExperienceBonusModifier { get; set; }

        [PacketField]
        public float DropBonusModifier { get; set; }

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
        public ulong ServerTime { get; set; }

        [PacketField]
        public bool EnablePlayerVersusPlayer { get; set; }

        [PacketField]
        public ulong ChatBanExpirationTime { get; set; }

        [PacketField]
        public uint TitleAchievementId { get; set; }

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
        public uint WeaponEnchantmentLevel { get; set; }

        [PacketField]
        public bool IsWorldEventTarget { get; set; }

        [PacketField]
        public int Unknown24 { get; set; }

        [PacketField]
        public byte Unknown25 { get; set; }

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
        public int Unknown26 { get; set; }

        [PacketField]
        public byte Unknown27 { get; set; }

        [PacketField]
        public long Unknown28 { get; set; }

        [PacketField]
        public int Unknown29 { get; set; }

        [PacketField]
        public float Scale { get; set; }

        [PacketField]
        public int GuildLogoId { get; set; }
    }
}
