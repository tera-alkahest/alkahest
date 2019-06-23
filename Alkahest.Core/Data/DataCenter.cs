using Alkahest.Core.IO;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Alkahest.Core.Data
{
    public sealed class DataCenter
    {
        public static readonly int KeySize = 16;

        public static readonly uint Version = 6;

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

        public static IReadOnlyDictionary<Region, uint> ClientVersions { get; } =
            new Dictionary<Region, uint>
            {
                { Region.DE, 350022 },
                { Region.FR, 350022 },
                { Region.JP, 350023 },
                { Region.KR, 0 },
                { Region.NA, 350027 },
                { Region.RU, 350024 },
                { Region.SE, 349932 },
                { Region.TH, 349932 },
                { Region.TW, 350025 },
                { Region.UK, 350022 },
            };

        const int UnknownSize = 8;

        const int AttributeSize = 8;

        const int ElementSize = 16;

        const int MetadataSize = 16;

        public DataCenterMode Mode { get; }

        public DataCenterHeader Header { get; }

        internal DataCenterSimpleRegion ElementExtensions { get; private set; }

        internal DataCenterSegmentedRegion Attributes { get; private set; }

        internal DataCenterSegmentedRegion Elements { get; private set; }

        internal DataCenterStringTable Values { get; private set; }

        internal DataCenterStringTable Names { get; private set; }

        public DataCenterFooter Footer { get; }

        public DataCenterElement Root => Materialize(DataCenterAddress.Zero);

        readonly ConcurrentDictionary<DataCenterAddress, DataCenterElement> _elements =
            new ConcurrentDictionary<DataCenterAddress, DataCenterElement>();

        readonly ConcurrentDictionary<DataCenterAddress, WeakReference<DataCenterElement>> _weakElements =
            new ConcurrentDictionary<DataCenterAddress, WeakReference<DataCenterElement>>();

        public bool IsFrozen => _frozen != null;

        object _frozen;

        public DataCenter(uint version)
        {
            Mode = DataCenterMode.Persistent;
            Header = new DataCenterHeader(Version, 0, 0, -16400, version, 0, 0, 0, 0);
            Footer = new DataCenterFooter(0);
        }

        public DataCenter(Stream stream, DataCenterMode mode, bool intern)
        {
            Mode = mode.CheckValidity(nameof(mode));

            using var reader = new GameBinaryReader(stream);

            Header = ReadHeader(reader);
            ElementExtensions = ReadSimpleRegion(reader, false, UnknownSize);
            Attributes = ReadSegmentedRegion(reader, AttributeSize);
            Elements = ReadSegmentedRegion(reader, ElementSize);
            Values = ReadStringTable(reader, 1024, intern);
            Names = ReadStringTable(reader, 512, intern);
            Footer = ReadFooter(reader);

            var diff = stream.Length - stream.Position;

            if (diff != 0)
                throw new InvalidDataException($"{diff} bytes remain unread.");
        }

        public object Freeze()
        {
            if (IsFrozen)
                throw new InvalidOperationException("Data center is already frozen.");

            return _frozen = new object();
        }

        public void Thaw(object token)
        {
            if (!IsFrozen)
                throw new InvalidOperationException("Data center is not frozen.");

            if (token != _frozen)
                throw new ArgumentException("Invalid freeze token.", nameof(token));

            _frozen = null;
        }

        internal DataCenterElement Materialize(DataCenterAddress address)
        {
            DataCenterElement Create(DataCenterAddress address)
            {
                return new DataCenterElement(this, address);
            }

            switch (Mode)
            {
                case DataCenterMode.Persistent:
                    return _elements.GetOrAdd(address, Create);
                case DataCenterMode.Transient:
                    return Create(address);
                case DataCenterMode.Weak:
                    var weak = _weakElements.GetOrAdd(address,
                        address => new WeakReference<DataCenterElement>(Create(address)));

                    if (!weak.TryGetTarget(out var elem))
                        weak.SetTarget(elem = Create(address));

                    return elem;
                default:
                    throw Assert.Unreachable();
            }
        }

        static DataCenterHeader ReadHeader(GameBinaryReader reader)
        {
            var version = reader.ReadUInt32();
            var unk1 = reader.ReadInt32();
            var unk2 = reader.ReadInt16();
            var unk3 = reader.ReadInt16();
            var clientVersion = reader.ReadUInt32();
            var unk4 = reader.ReadInt32();
            var unk5 = reader.ReadInt32();
            var unk6 = reader.ReadInt32();
            var unk7 = reader.ReadInt32();

            return new DataCenterHeader(version, unk1, unk2, unk3, clientVersion, unk4, unk5, unk6, unk7);
        }

        static DataCenterFooter ReadFooter(GameBinaryReader reader)
        {
            var unk1 = reader.ReadInt32();

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

        static DataCenterSegmentedSimpleRegion ReadSegmentedSimpleRegion(GameBinaryReader reader,
            uint count, uint elementSize)
        {
            var segments = new List<DataCenterSimpleRegion>((int)count);

            for (var i = 0; i < count; i++)
                segments.Add(ReadSimpleRegion(reader, false, elementSize));

            return new DataCenterSegmentedSimpleRegion(elementSize, segments);
        }

        static DataCenterRegion ReadRegion(GameBinaryReader reader, uint elementSize)
        {
            var full = reader.ReadUInt32();
            var used = reader.ReadUInt32();
            var data = reader.ReadBytes((int)(full * elementSize));

            return new DataCenterRegion(elementSize, full, used, data);
        }

        static DataCenterSegmentedRegion ReadSegmentedRegion(GameBinaryReader reader, uint elementSize)
        {
            var count = reader.ReadUInt32();
            var segments = new List<DataCenterRegion>((int)count);

            for (var i = 0; i < count; i++)
                segments.Add(ReadRegion(reader, elementSize));

            return new DataCenterSegmentedRegion(elementSize, segments);
        }

        static unsafe DataCenterStringTable ReadStringTable(GameBinaryReader reader, uint count,
            bool intern)
        {
            var strings = ReadSegmentedRegion(reader, sizeof(char));
            var metadata = ReadSegmentedSimpleRegion(reader, count, MetadataSize);
            var addresses = ReadSimpleRegion(reader, true, (uint)sizeof(DataCenterAddress));

            return new DataCenterStringTable(strings, metadata, addresses, intern);
        }

        internal static DataCenterAddress ReadAddress(GameBinaryReader reader)
        {
            var segment = reader.ReadUInt16();
            var element = reader.ReadUInt16();

            return new DataCenterAddress(segment, element);
        }
    }
}
