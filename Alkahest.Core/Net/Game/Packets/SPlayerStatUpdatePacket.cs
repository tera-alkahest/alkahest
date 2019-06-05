using Alkahest.Core.Game;
using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SPlayerStatUpdatePacket : Packet
    {
        const string Name = "S_PLAYER_STAT_UPDATE";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SPlayerStatUpdatePacket();
        }

        [PacketField]
        public ulong CurrentHP { get; set; }

        [PacketField]
        public uint CurrentMP { get; set; }

        [PacketField]
        public long Unknown1 { get; set; }

        [PacketField]
        public ulong MaxHP { get; set; }

        [PacketField]
        public uint MaxMP { get; set; }

        [PacketField]
        public uint BasePower { get; set; }

        [PacketField]
        public uint BaseEndurance { get; set; }

        [PacketField]
        public uint BaseImpactFactor { get; set; }

        [PacketField]
        public uint BaseBalanceFactor { get; set; }

        [PacketField]
        public ushort BaseWalkSpeed { get; set; }

        [PacketField]
        public ushort BaseRunSpeed { get; set; }

        [PacketField]
        public ushort BaseAttackSpeed { get; set; }

        [PacketField]
        public float BaseCritFactor { get; set; }

        [PacketField]
        public float BaseCritResistFactor { get; set; }

        [PacketField]
        public float BaseCritPower { get; set; }

        [PacketField]
        public uint MinBaseAttack { get; set; }

        [PacketField]
        public uint MaxBaseAttack { get; set; }

        [PacketField]
        public uint BaseDefense { get; set; }

        [PacketField]
        public uint BaseImpact { get; set; }

        [PacketField]
        public uint BaseBalance { get; set; }

        [PacketField]
        public float BaseWeakeningResistFactor { get; set; }

        [PacketField]
        public float BasePeriodicResistFactor { get; set; }

        [PacketField]
        public float BaseStunResistFactor { get; set; }

        [PacketField]
        public uint BonusPower { get; set; }

        [PacketField]
        public uint BonusEndurance { get; set; }

        [PacketField]
        public uint BonusImpactFactor { get; set; }

        [PacketField]
        public uint BonusBalanceFactor { get; set; }

        [PacketField]
        public ushort BonusWalkSpeed { get; set; }

        [PacketField]
        public ushort BonusRunSpeed { get; set; }

        [PacketField]
        public ushort BonusAttackSpeed { get; set; }

        [PacketField]
        public float BonusCritFactor { get; set; }

        [PacketField]
        public float BonusCritResistFactor { get; set; }

        [PacketField]
        public float BonusCritPower { get; set; }

        [PacketField]
        public uint BonusMinAttack { get; set; }

        [PacketField]
        public uint BonusMaxAttack { get; set; }

        [PacketField]
        public uint BonusDefense { get; set; }

        [PacketField]
        public uint BonusImpact { get; set; }

        [PacketField]
        public uint BonusBalance { get; set; }

        [PacketField]
        public float BonusWeakeningResistFactor { get; set; }

        [PacketField]
        public float BonusPeriodicResistFactor { get; set; }

        [PacketField]
        public float BonusStunResistFactor { get; set; }

        [PacketField]
        public ushort Level { get; set; }

        [PacketField]
        public UserStatus Status { get; set; }

        [PacketField]
        public ushort StaminaLevel { get; set; }

        [PacketField]
        public bool IsAlive { get; set; }

        [PacketField]
        public uint StaminaBonusHP { get; set; }

        [PacketField]
        public uint StaminaBonusMP { get; set; }

        [PacketField]
        public uint CurrentStamina { get; set; }

        [PacketField]
        public uint MaxStamina { get; set; }

        [PacketField]
        public uint CurrentResource { get; set; }

        [PacketField]
        public uint MaxResource { get; set; }

        [PacketField]
        public uint BonusResource { get; set; }

        [PacketField]
        public int Unknown2 { get; set; }

        [PacketField]
        public uint InventoryItemLevel { get; set; }

        [PacketField]
        public uint CharacterItemLevel { get; set; }

        [PacketField]
        public uint EdgeStacks { get; set; }

        [PacketField]
        public float Unknown3 { get; set; }

        [PacketField]
        public uint EdgeTimeRemaining { get; set; }

        [PacketField]
        public int Unknown4 { get; set; }

        [PacketField]
        public uint RealLevel { get; set; }

        [PacketField]
        public float FlightEnergy { get; set; }

        [PacketField]
        public int Unknown5 { get; set; }

        [PacketField]
        public float Unknown6 { get; set; }

        [PacketField]
        public uint FireStacks { get; set; }

        [PacketField]
        public uint IceStacks { get; set; }

        [PacketField]
        public uint ArcaneStacks { get; set; }

        [PacketField]
        public uint AdventureCoins { get; set; }

        [PacketField]
        public uint MaxAdventureCoins { get; set; }
    }
}
