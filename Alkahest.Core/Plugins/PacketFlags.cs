using System;

namespace Alkahest.Core.Plugins
{
    [Flags]
    public enum PacketFlags
    {
        None = 0b000,
        Forged = 0b001,
        Modified = 0b010,
        Silenced = 0b100,
    }
}
