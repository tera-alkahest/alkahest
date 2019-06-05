using Alkahest.Core.Net.Game.Serialization;

namespace Alkahest.Core.Net.Game.Packets
{
    [Packet("S_PLAYER_CHANGE_FLIGHT_ENERGY")]
    public sealed class SPlayerChangeFlightEnergyPacket : SerializablePacket
    {
        public float FlightEnergy { get; set; }
    }
}
