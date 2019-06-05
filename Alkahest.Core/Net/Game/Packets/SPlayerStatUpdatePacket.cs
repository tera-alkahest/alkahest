using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_PLAYER_STAT_UPDATE")]
    public sealed class SPlayerStatUpdatePacket : SerializablePacket
    {
        public ulong CurrentHP { get; set; }

        public uint CurrentMP { get; set; }

        public long Unknown1 { get; set; }

        public ulong MaxHP { get; set; }

        public uint MaxMP { get; set; }

        public uint BasePower { get; set; }

        public uint BaseEndurance { get; set; }

        public uint BaseImpactFactor { get; set; }

        public uint BaseBalanceFactor { get; set; }

        public ushort BaseWalkSpeed { get; set; }

        public ushort BaseRunSpeed { get; set; }

        public ushort BaseAttackSpeed { get; set; }

        public float BaseCritFactor { get; set; }

        public float BaseCritResistFactor { get; set; }

        public float BaseCritPower { get; set; }

        public uint MinBaseAttack { get; set; }

        public uint MaxBaseAttack { get; set; }

        public uint BaseDefense { get; set; }

        public uint BaseImpact { get; set; }

        public uint BaseBalance { get; set; }

        public float BaseWeakeningResistFactor { get; set; }

        public float BasePeriodicResistFactor { get; set; }

        public float BaseStunResistFactor { get; set; }

        public uint BonusPower { get; set; }

        public uint BonusEndurance { get; set; }

        public uint BonusImpactFactor { get; set; }

        public uint BonusBalanceFactor { get; set; }

        public ushort BonusWalkSpeed { get; set; }

        public ushort BonusRunSpeed { get; set; }

        public ushort BonusAttackSpeed { get; set; }

        public float BonusCritFactor { get; set; }

        public float BonusCritResistFactor { get; set; }

        public float BonusCritPower { get; set; }

        public uint BonusMinAttack { get; set; }

        public uint BonusMaxAttack { get; set; }

        public uint BonusDefense { get; set; }

        public uint BonusImpact { get; set; }

        public uint BonusBalance { get; set; }

        public float BonusWeakeningResistFactor { get; set; }

        public float BonusPeriodicResistFactor { get; set; }

        public float BonusStunResistFactor { get; set; }

        public ushort Level { get; set; }

        public UserStatus Status { get; set; }

        public ushort StaminaLevel { get; set; }

        public bool IsAlive { get; set; }

        public uint StaminaBonusHP { get; set; }

        public uint StaminaBonusMP { get; set; }

        public uint CurrentStamina { get; set; }

        public uint MaxStamina { get; set; }

        public uint CurrentResource { get; set; }

        public uint MaxResource { get; set; }

        public uint BonusResource { get; set; }

        public int Unknown2 { get; set; }

        public uint InventoryItemLevel { get; set; }

        public uint CharacterItemLevel { get; set; }

        public uint EdgeStacks { get; set; }

        public float Unknown3 { get; set; }

        public uint EdgeTimeRemaining { get; set; }

        public int Unknown4 { get; set; }

        public uint RealLevel { get; set; }

        public float FlightEnergy { get; set; }

        public int Unknown5 { get; set; }

        public float Unknown6 { get; set; }

        public uint FireStacks { get; set; }

        public uint IceStacks { get; set; }

        public uint ArcaneStacks { get; set; }

        public uint AdventureCoins { get; set; }

        public uint MaxAdventureCoins { get; set; }
    }
}
