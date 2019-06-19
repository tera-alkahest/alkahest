using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Alkahest.Core.Data
{
    sealed class DataCenterStringTable
    {
        public IReadOnlyList<DataCenterString> ByIndex { get; }

        public IReadOnlyDictionary<DataCenterAddress, DataCenterString> ByAddress { get; }

        public DataCenterStringTable(DataCenterSegmentedRegion strings,
            DataCenterSegmentedSimpleRegion metadata, DataCenterSimpleRegion addresses, bool intern)
        {
            var list = new List<DataCenterString>((int)addresses.Count);

            foreach (var segment in metadata.Segments)
            {
                for (uint i = 0; i < segment.Count; i++)
                {
                    var reader = segment.GetReader(i);

                    // This hash only has a tiny amount of collisions in a typical data center.
                    var hash = reader.ReadUInt32();

                    // This includes the NUL character.
                    var length = reader.ReadUInt32();

                    var index = reader.ReadUInt32() - 1;
                    var address1 = DataCenter.ReadAddress(reader);
                    var address2 = DataCenter.ReadAddress(addresses.GetReader(index));

                    if (address1 != address2)
                        throw new InvalidDataException();

                    var value = strings.GetReader(address1).ReadString();

                    if (value.Length != length - 1)
                        throw new InvalidDataException();

                    list.Add(new DataCenterString(index, address1, intern ?
                        string.Intern(value) : value, hash));
                }
            }

            ByIndex = list.OrderBy(x => x.Index).ToArray();
            ByAddress = list.ToDictionary(x => x.Address);
        }
    }
}
