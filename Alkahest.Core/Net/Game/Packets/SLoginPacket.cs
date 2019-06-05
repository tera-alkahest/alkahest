using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;
using System.Collections.Generic;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_LOGIN")]
    public sealed class SLoginPacket : SerializablePacket
    {
        public sealed class ServantInfo
        {
            public long Unknown1 { get; set; }

            public int Unknown2 { get; set; }

            public int Unknown3 { get; set; }

            public int Unknown4 { get; set; }

            public int Unknown5 { get; set; }
        }

        public List<ServantInfo> Servants { get; } = new List<ServantInfo>();

        public string UserName { get; set; }

        public List<byte> Details { get; } = new List<byte>();

        public List<byte> Shape { get; } = new List<byte>();

        public TemplateId Template { get; set; }

        public GameId Target { get; set; }

        public uint ServerId { get; set; }

        public uint PlayerId { get; set; }

        public int Unknown6 { get; set; }

        public bool IsAlive { get; set; }

        public int Unknown7 { get; set; }

        public uint WalkSpeed { get; set; }

        public uint RunSpeed { get; set; }

        public Appearance Appearance { get; set; }

        public bool IsVisible { get; set; }

        public bool IsSecondCharacter { get; set; }

        public ushort Level { get; set; }

        public uint ApexLevel { get; set; }

        public uint OreGatheringLevel { get; set; }

        public int Unknown8 { get; set; }

        public uint PlantGatheringLevel { get; set; }

        public uint EnergyGatheringLevel { get; set; }

        public short Unknown9 { get; set; }

        public int Unknown10 { get; set; }

        public int Unknown11 { get; set; }

        public ulong TotalExperience { get; set; }

        public ulong ShownExperience { get; set; }

        public ulong NeededExperience { get; set; }

        public uint EnhancementLevel { get; set; }

        public ulong EnhancementExperience { get; set; }

        public uint EnhancementDailyExperience { get; set; }

        public uint CurrentRestedExperience { get; set; }

        public uint MaxRestedExperience { get; set; }

        public float ExperienceBonusModifier { get; set; }

        public float DropBonusModifier { get; set; }

        public uint WeaponItemId { get; set; }

        public uint ArmorItemId { get; set; }

        public uint HandwearItemId { get; set; }

        public uint FootwearItemId { get; set; }

        public uint UnderwearItemId { get; set; }

        public uint CircletItemId { get; set; }

        public uint MaskItemId { get; set; }

        public ulong ServerTime { get; set; }

        public bool EnablePlayerVersusPlayer { get; set; }

        public ulong ChatBanExpirationTime { get; set; }

        public uint TitleAchievementId { get; set; }

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

        public uint WeaponEnchantmentLevel { get; set; }

        public bool IsWorldEventTarget { get; set; }

        public int Unknown24 { get; set; }

        public byte Unknown25 { get; set; }

        public uint HairCostumeItemId { get; set; }

        public uint FaceCostumeItemId { get; set; }

        public uint BackCostumeItemId { get; set; }

        public uint WeaponSkinItemId { get; set; }

        public uint BodyCostumeItemId { get; set; }

        public uint FootprintItemId { get; set; }

        public int Unknown26 { get; set; }

        public byte Unknown27 { get; set; }

        public long Unknown28 { get; set; }

        public int Unknown29 { get; set; }

        public float Scale { get; set; }

        public int GuildLogoId { get; set; }
    }
}
