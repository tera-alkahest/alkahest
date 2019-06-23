using Alkahest.Core.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Alkahest.Core.Data
{
    sealed class DataCenterStringTable
    {
        public IReadOnlyList<string> ByIndex { get; }

        public IReadOnlyDictionary<DataCenterAddress, string> ByAddress { get; }

        public DataCenterStringTable(DataCenterSegmentedRegion strings,
            DataCenterSegmentedSimpleRegion table, DataCenterSimpleRegion addresses, bool intern)
        {
            var list = new List<(uint, DataCenterAddress, string)>((int)addresses.Count);

            foreach (var (i, segment) in table.Segments.WithIndex())
            {
                for (uint j = 0; j < segment.Count; j++)
                {
                    var reader = segment.GetReader(j);

                    // This hash only has a tiny amount of collisions in a typical data center.
                    var hash = reader.ReadUInt32();
                    var bucket = (hash ^ hash >> 16) % table.Segments.Count;

                    if (bucket != i)
                        throw new InvalidDataException(
                            $"String bucket index {i} does not match expected index {bucket}.");

                    // This includes the NUL character.
                    var length = reader.ReadUInt32() - 1;

                    var index = reader.ReadUInt32() - 1;
                    var address1 = DataCenter.ReadAddress(reader);
                    var address2 = DataCenter.ReadAddress(addresses.GetReader(index));

                    if (address1 != address2)
                        throw new InvalidDataException(
                            $"String address {address1} does not match expected address {address2}.");

                    var value = strings.GetReader(address1).ReadString();

                    if (value.Length != length)
                        throw new InvalidDataException(
                            $"String length {value.Length} does not match recorded length {length}.");

                    list.Add((index, address1, intern ? string.Intern(value) : value));
                }
            }

            ByIndex = list.OrderBy(x => x.Item1).Select(x => x.Item3).ToArray();
            ByAddress = list.ToDictionary(x => x.Item2, x => x.Item3);
        }
    }
}
