namespace Alkahest.Core.Net.Game.Packets
{
    public sealed class SPlayerChangeFlightEnergyPacket : Packet
    {
        const string Name = "S_PLAYER_CHANGE_FLIGHT_ENERGY";

        public override string OpCode => Name;

        [Packet(Name)]
        internal static Packet Create()
        {
            return new SPlayerChangeFlightEnergyPacket();
        }

        [PacketField]
        public float FlightEnergy { get; set; }
    }
}
