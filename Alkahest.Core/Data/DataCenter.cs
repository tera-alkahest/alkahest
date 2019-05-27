using Alkahest.Core.IO;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Alkahest.Core.Data
{
    public sealed class DataCenter : IDisposable
    {
        public const int KeySize = 16;

        const int Unknown1Size = 8;

        const int AttributeSize = 8;

        const int ElementSize = 16;

        const int Unknown2Size = 16;

        public DataCenterHeader Header { get; }

        public DataCenterFooter Footer { get; }

        public DataCenterElement Root { get; }

        internal DataCenterSegmentedRegion Attributes { get; private set; }

        internal DataCenterSegmentedRegion Elements { get; private set; }

        internal IReadOnlyList<string> Names { get; private set; }

        internal bool IsDisposed { get; private set; }

        internal ReaderWriterLockSlim Lock { get; } = new ReaderWriterLockSlim();

        ConcurrentDictionary<DataCenterAddress, string> _strings =
            new ConcurrentDictionary<DataCenterAddress, string>();

        DataCenterSegmentedRegion _stringRegion;

        public DataCenter(string fileName)
        {
            DataCenterSegmentedRegion attributeRegion;
            DataCenterSegmentedRegion elementRegion;
            DataCenterSegmentedRegion nameRegion;
            DataCenterSimpleRegion nameAddressRegion;

            using (var reader = new TeraBinaryReader(File.OpenRead(fileName)))
            {
                Header = ReadHeader(reader);

                ReadRegions(reader, out attributeRegion, out elementRegion,
                    out _stringRegion, out nameRegion, out nameAddressRegion);

                Footer = ReadFooter(reader);
            }

            Attributes = attributeRegion;
            Elements = elementRegion;
            Names = ReadAddresses(nameAddressRegion)
                .Select(x => ReadString(nameRegion, x))
                .ToArray();

            Root = new DataCenterElement(this, DataCenterAddress.Zero);
        }

        public void Dispose()
        {
            try
            {
                Lock.EnterWriteLock();

                IsDisposed = true;

                Attributes = null;
                Elements = null;
                Names = null;
                _strings = null;
                _stringRegion = null;
            }
            finally
            {
                Lock.ExitWriteLock();
            }
        }

        internal string GetString(DataCenterAddress address)
        {
            return _strings.GetOrAdd(address, a => _stringRegion.GetReader(address).ReadString());
        }

        static DataCenterHeader ReadHeader(TeraBinaryReader reader)
        {
            var unk1 = reader.ReadUInt32();
            var unk2 = reader.ReadUInt32();
            var unk3 = reader.ReadUInt32();
            var version = reader.ReadUInt32();
            var unk4 = reader.ReadUInt32();
            var unk5 = reader.ReadUInt32();
            var unk6 = reader.ReadUInt32();
            var unk7 = reader.ReadUInt32();

            return new DataCenterHeader(unk1, unk2, unk3, version, unk4, unk5, unk6, unk7);
        }

        static unsafe void ReadRegions(TeraBinaryReader reader,
            out DataCenterSegmentedRegion attributeRegion,
            out DataCenterSegmentedRegion elementRegion,
            out DataCenterSegmentedRegion stringRegion,
            out DataCenterSegmentedRegion nameRegion,
            out DataCenterSimpleRegion nameAddressRegion)
        {
            ReadSimpleRegion(reader, false, Unknown1Size);

            attributeRegion = ReadSegmentedRegion(reader, AttributeSize);
            elementRegion = ReadSegmentedRegion(reader, ElementSize);

            stringRegion = ReadSegmentedRegion(reader, sizeof(char));
            ReadSimpleSegmentedRegion(reader, 1024, Unknown2Size);
            ReadSimpleRegion(reader, true, (uint)sizeof(DataCenterAddress));

            nameRegion = ReadSegmentedRegion(reader, sizeof(char));
            ReadSimpleSegmentedRegion(reader, 512, Unknown2Size);
            nameAddressRegion = ReadSimpleRegion(reader, true, (uint)sizeof(DataCenterAddress));
        }

        static DataCenterFooter ReadFooter(TeraBinaryReader reader)
        {
            var unk1 = reader.ReadUInt32();

            return new DataCenterFooter(unk1);
        }

        static DataCenterSimpleRegion ReadSimpleRegion(
            TeraBinaryReader reader, bool offByOne, uint elementSize)
        {
            var count = reader.ReadUInt32();

            if (offByOne)
                count--;

            var data = reader.ReadBytes((int)(count * elementSize));

            return new DataCenterSimpleRegion(elementSize, count, data);
        }

        static DataCenterSimpleSegmentedRegion ReadSimpleSegmentedRegion(
            TeraBinaryReader reader, uint count, uint elementSize)
        {
            var segments = new List<DataCenterSimpleRegion>();

            for (var i = 0; i < count; i++)
                segments.Add(ReadSimpleRegion(reader, false, elementSize));

            return new DataCenterSimpleSegmentedRegion(elementSize, segments);
        }

        static DataCenterSegment ReadSegment(TeraBinaryReader reader, uint elementSize)
        {
            var full = reader.ReadUInt32();
            var used = reader.ReadUInt32();
            var data = reader.ReadBytes((int)(full * elementSize));

            return new DataCenterSegment(elementSize, full, used, data);
        }

        static DataCenterSegmentedRegion ReadSegmentedRegion(
            TeraBinaryReader reader, uint elementSize)
        {
            var segments = new List<DataCenterSegment>();
            var count = reader.ReadUInt32();

            for (var i = 0; i < count; i++)
                segments.Add(ReadSegment(reader, elementSize));

            return new DataCenterSegmentedRegion(elementSize, segments);
        }

        internal static DataCenterAddress ReadAddress(TeraBinaryReader reader)
        {
            var segment = reader.ReadUInt16();
            var element = reader.ReadUInt16();

            return new DataCenterAddress(segment, element);
        }

        static IEnumerable<DataCenterAddress> ReadAddresses(DataCenterSimpleRegion region)
        {
            var reader = region.GetReader(0);

            for (var i = 0; i < region.Count; i++)
                yield return ReadAddress(reader);
        }

        static string ReadString(DataCenterSegmentedRegion region, DataCenterAddress address)
        {
            return region.GetReader(address).ReadString();
        }
    }
}
