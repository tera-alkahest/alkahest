using System;

namespace Alkahest.Core.Data
{
    [Flags]
    public enum DataCenterStringOptions
    {
        None = 0b00,
        Intern = 0b01,
        Lazy = 0b10,
    }
}
