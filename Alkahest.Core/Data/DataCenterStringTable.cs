using Alkahest.Core.Collections;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace Alkahest.Core.Data
{
    sealed class DataCenterStringTable
    {
        readonly DataCenterSegmentedRegion _data;

        readonly bool _intern;

        readonly IReadOnlyList<DataCenterAddress> _addresses;

        readonly ConcurrentDictionary<DataCenterAddress, string> _strings;

        public DataCenterStringTable(DataCenterSegmentedRegion data,
            DataCenterSegmentedSimpleRegion table, DataCenterSimpleRegion addresses,
            DataCenterStringOptions options)
        {
            _intern = options.HasFlag(DataCenterStringOptions.Intern);

            var count = (int)addresses.Count;
            var addrs = new List<DataCenterAddress>(count);

            for (uint i = 0; i < addresses.Count; i++)
                addrs.Add(DataCenter.ReadAddress(addresses.GetReader(i)));

            _addresses = addrs;
            _strings = new ConcurrentDictionary<DataCenterAddress, string>(
                Environment.ProcessorCount, count);

            if (!options.HasFlag(DataCenterStringOptions.Lazy))
            {
                foreach (var (i, segment) in table.Segments.WithIndex())
                {
                    for (uint j = 0; j < segment.Count; j++)
                    {
                        var reader = segment.GetReader(j);

                        // This hash only has a tiny amount of collisions in a
                        // typical data center.
                        var hash = reader.ReadUInt32();
                        var bucket = (hash ^ hash >> 16) % table.Segments.Count;

                        if (bucket != i)
                            throw new InvalidDataException(
                                $"String bucket index {i} does not match expected index {bucket}.");

                        // This includes the NUL character.
                        var length = reader.ReadUInt32() - 1;

                        var index = reader.ReadUInt32() - 1;

                        if (index >= addrs.Count)
                            throw new InvalidDataException(
                                $"String index {index} is greater than {addrs.Count}.");

                        var address1 = DataCenter.ReadAddress(reader);
                        var address2 = _addresses[(int)index];

                        if (address1 != address2)
                            throw new InvalidDataException(
                                $"String address {address1} does not match expected address {address2}.");

                        var value = data.GetReader(address1).ReadString();

                        if (value.Length != length)
                            throw new InvalidDataException(
                                $"String length {value.Length} does not match recorded length {length}.");

                        if (!_strings.TryAdd(address1, _intern ? string.Intern(value) : value))
                            throw new InvalidDataException(
                                $"String address {address1} already added to the table.");
                    }
                }
            }
            else
                _data = data;
        }

        public string Get(int index)
        {
            if (index >= _addresses.Count)
                throw new InvalidDataException($"String index {index} is greater than {_addresses.Count}.");

            return Get(_addresses[index]);
        }

        public string Get(DataCenterAddress address)
        {
            return _strings.GetOrAdd(address, a =>
            {
                var str = _data.GetReader(a).ReadString();

                return _intern ? string.Intern(str) : str;
            });
        }
    }
}
