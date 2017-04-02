namespace Alkahest.Core.Net.Protocol.Packets
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
        public uint CurrentHP { get; set; }

        [PacketField]
        public uint CurrentMP { get; set; }

        [PacketField]
        public uint Unknown1 { get; set; }

        [PacketField]
        public uint MaxHP { get; set; }

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
        public ushort BaseMovementSpeed { get; set; }

        [PacketField]
        public ushort Unknown2 { get; set; }

        [PacketField]
        public ushort BaseAttackSpeed { get; set; }

        [PacketField]
        public float BaseCritFactor { get; set; }

        [PacketField]
        public float BaseCritResistFactor { get; set; }

        [PacketField]
        public float BaseCritPower { get; set; }

        [PacketField]
        public uint BaseAttack1 { get; set; }

        [PacketField]
        public uint BaseAttack2 { get; set; }

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
        public ushort BonusMovementSpeed { get; set; }

        [PacketField]
        public ushort Unknown3 { get; set; }

        [PacketField]
        public ushort BonusAttackSpeed { get; set; }

        [PacketField]
        public float BonusCritFactor { get; set; }

        [PacketField]
        public float BonusCritResistFactor { get; set; }

        [PacketField]
        public float BonusCritPower { get; set; }

        [PacketField]
        public uint BonusAttack1 { get; set; }

        [PacketField]
        public uint BonusAttack2 { get; set; }

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
        public ushort Level1 { get; set; }

        [PacketField]
        public byte Unknown4 { get; set; }

        [PacketField]
        public uint Unknown5 { get; set; }

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
        public uint Unknown7 { get; set; }

        [PacketField]
        public uint InventoryItemLevel { get; set; }

        [PacketField]
        public uint CharacterItemLevel { get; set; }

        [PacketField]
        public uint ResourceStacks { get; set; }

        [PacketField]
        public ushort Unknown8 { get; set; }

        [PacketField]
        public ushort Unknown9 { get; set; }

        [PacketField]
        public uint Unknown10 { get; set; }

        [PacketField]
        public uint Unknown11 { get; set; }

        [PacketField]
        public uint Level2 { get; set; }

        [PacketField]
        public float FlightEnergy { get; set; }

        [PacketField]
        public uint Unknown13 { get; set; }

        [PacketField]
        public float Unknown14 { get; set; }
    }
}
