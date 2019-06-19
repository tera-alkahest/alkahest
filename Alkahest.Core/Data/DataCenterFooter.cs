using System.IO;

namespace Alkahest.Core.Data
{
    public sealed class DataCenterFooter
    {
        public int Unknown1 { get; }

        internal DataCenterFooter(int unknown1)
        {
            if (unknown1 != 0)
                throw new InvalidDataException($"Unexpected Unknown1 value {unknown1} in footer.");

            Unknown1 = unknown1;
        }
    }
}
