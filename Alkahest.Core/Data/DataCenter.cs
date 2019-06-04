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

        public static IReadOnlyDictionary<Region, string> FileNames { get; } =
            new Dictionary<Region, string>
            {
                { Region.DE, "DataCenter_Final_GER.dat" },
                { Region.FR, "DataCenter_Final_FRA.dat" },
                { Region.JP, "DataCenter_Final_JPN.dat" },
                { Region.KR, "DataCenter_Final.dat" },
                { Region.NA, "DataCenter_Final_USA.dat" },
                { Region.RU, "DataCenter_Final_RUS.dat" },
                { Region.SE, "DataCenter_Final_SE.dat" },
                { Region.TH, "DataCenter_Final_THA.dat" },
                { Region.TW, "DataCenter_Final_TW.dat" },
                { Region.UK, "DataCenter_Final_EUR.dat" },
            };

        const int Unknown1Size = 8;

        const int AttributeSize = 8;

        const int ElementSize = 16;

        const int Unknown2Size = 16;

        public DataCenterHeader Header { get; }

        public DataCenterFooter Footer { get; }

        public DataCenterElement Root { get; private set; }

        internal DataCenterSegmentedRegion Attributes { get; private set; }

        internal DataCenterSegmentedRegion Elements { get; private set; }

        internal IReadOnlyList<string> Names { get; private set; }

        internal bool IsFrozen { get; private set; }

        internal bool IsDisposed { get; private set; }

        internal ReaderWriterLockSlim Lock { get; } = new ReaderWriterLockSlim();

        ConcurrentDictionary<DataCenterAddress, string> _strings =
            new ConcurrentDictionary<DataCenterAddress, string>();

        DataCenterSegmentedRegion _stringRegion;

        public DataCenter()
        {
            Header = new DataCenterHeader(0, 0, 0, 0, 0, 0, 0, 0);
            Footer = new DataCenterFooter(0);
            Root = new DataCenterElement(this, DataCenterAddress.Zero);
        }

        public unsafe DataCenter(string fileName)
        {
            using var reader = new GameBinaryReader(File.OpenRead(fileName));

            Header = ReadHeader(reader);

            ReadSimpleRegion(reader, false, Unknown1Size);

            var attributeRegion = ReadSegmentedRegion(reader, AttributeSize);
            var elementRegion = ReadSegmentedRegion(reader, ElementSize);

            _stringRegion = ReadSegmentedRegion(reader, sizeof(char));

            ReadSimpleSegmentedRegion(reader, 1024, Unknown2Size);
            ReadSimpleRegion(reader, true, (uint)sizeof(DataCenterAddress));

            var nameRegion = ReadSegmentedRegion(reader, sizeof(char));

            ReadSimpleSegmentedRegion(reader, 512, Unknown2Size);

            var nameAddressRegion = ReadSimpleRegion(reader, true, (uint)sizeof(DataCenterAddress));

            Footer = ReadFooter(reader);
            Attributes = attributeRegion;
            Elements = elementRegion;
            Names = ReadAddresses(nameAddressRegion).Select(x => ReadString(nameRegion, x)).ToArray();

            Reset();
        }

        public void Dispose()
        {
            if (IsFrozen)
                throw new InvalidOperationException("Data center is frozen.");

            try
            {
                Lock.EnterWriteLock();

                IsDisposed = true;

                Root = null;
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

        internal void Freeze()
        {
            IsFrozen = true;
        }

        internal void Thaw()
        {
            IsFrozen = false;
        }

        public void Reset()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().FullName);

            if (IsFrozen)
                throw new InvalidOperationException("Data center is frozen.");

            Root = new DataCenterElement(this, DataCenterAddress.Zero);
        }

        internal string GetString(DataCenterAddress address)
        {
            return _strings.GetOrAdd(address, a => _stringRegion.GetReader(address).ReadString());
        }

        static DataCenterHeader ReadHeader(GameBinaryReader reader)
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

        static DataCenterFooter ReadFooter(GameBinaryReader reader)
        {
            var unk1 = reader.ReadUInt32();

            return new DataCenterFooter(unk1);
        }

        static DataCenterSimpleRegion ReadSimpleRegion(GameBinaryReader reader, bool offByOne,
            uint elementSize)
        {
            var count = reader.ReadUInt32();

            if (offByOne)
                count--;

            var data = reader.ReadBytes((int)(count * elementSize));

            return new DataCenterSimpleRegion(elementSize, count, data);
        }

        static DataCenterSimpleSegmentedRegion ReadSimpleSegmentedRegion(GameBinaryReader reader,
            uint count, uint elementSize)
        {
            var segments = new List<DataCenterSimpleRegion>();

            for (var i = 0; i < count; i++)
                segments.Add(ReadSimpleRegion(reader, false, elementSize));

            return new DataCenterSimpleSegmentedRegion(elementSize, segments);
        }

        static DataCenterSegment ReadSegment(GameBinaryReader reader, uint elementSize)
        {
            var full = reader.ReadUInt32();
            var used = reader.ReadUInt32();
            var data = reader.ReadBytes((int)(full * elementSize));

            return new DataCenterSegment(elementSize, full, used, data);
        }

        static DataCenterSegmentedRegion ReadSegmentedRegion(GameBinaryReader reader, uint elementSize)
        {
            var count = reader.ReadUInt32();
            var segments = new List<DataCenterSegment>((int)count);

            for (var i = 0; i < count; i++)
                segments.Add(ReadSegment(reader, elementSize));

            return new DataCenterSegmentedRegion(elementSize, segments);
        }

        internal static DataCenterAddress ReadAddress(GameBinaryReader reader)
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
