using Alkahest.Core.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Alkahest.Core.Data
{
    public sealed class DataCenter : IDisposable
    {
        public const int KeySize = 16;

        public const int Version = 6;

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

        public DataCenterHeader Header { get; }

        internal DataCenterSimpleRegion Unknown { get; private set; }

        internal DataCenterSegmentedRegion Attributes { get; private set; }

        internal DataCenterSegmentedRegion Elements { get; private set; }

        internal DataCenterStringTable Values { get; private set; }

        internal DataCenterStringTable Names { get; private set; }

        public DataCenterFooter Footer { get; }

        public DataCenterElement Root { get; private set; }

        public bool IsFrozen => _frozen != null;

        internal bool IsDisposed { get; private set; }

        internal ReaderWriterLockSlim Lock { get; } = new ReaderWriterLockSlim();

        object _frozen;

        public DataCenter(uint version)
        {
            Header = new DataCenterHeader(Version, 0, 0, -16400, version, 0, 0, 0, 0);
            Footer = new DataCenterFooter(0);
            Root = new DataCenterElement(this, DataCenterAddress.Zero);
        }

        public DataCenter(Stream stream, bool intern)
        {
            using var reader = new GameBinaryReader(stream);

            Header = ReadHeader(reader);
            Unknown = ReadSimpleRegion(reader, false, UnknownSize);
            Attributes = ReadSegmentedRegion(reader, AttributeSize);
            Elements = ReadSegmentedRegion(reader, ElementSize);
            Values = ReadStringTable(reader, 1024, intern);
            Names = ReadStringTable(reader, 512, intern);
            Footer = ReadFooter(reader);

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

                Unknown = null;
                Attributes = null;
                Elements = null;
                Values = null;
                Names = null;
                Root = null;
            }
            finally
            {
                Lock.ExitWriteLock();
            }
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

            if (_frozen != token)
                throw new ArgumentException("Invalid freeze token.", nameof(token));

            _frozen = null;
        }

        public void Reset()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().FullName);

            if (IsFrozen)
                throw new InvalidOperationException("Data center is frozen.");

            Root = new DataCenterElement(this, DataCenterAddress.Zero);
        }

        static DataCenterHeader ReadHeader(GameBinaryReader reader)
        {
            var version = reader.ReadUInt32();

            if (version != Version)
                throw new InvalidDataException();

            var unk1 = reader.ReadInt32();

            if (unk1 != 0)
                throw new InvalidDataException();

            var unk2 = reader.ReadInt16();

            if (unk2 != 0)
                throw new InvalidDataException();

            var unk3 = reader.ReadInt16();

            if (unk3 != -16400)
                throw new InvalidDataException();

            var clientVersion = reader.ReadUInt32();
            var unk4 = reader.ReadInt32();

            if (unk4 != 0)
                throw new InvalidDataException();

            var unk5 = reader.ReadInt32();

            if (unk5 != 0)
                throw new InvalidDataException();

            var unk6 = reader.ReadInt32();

            if (unk6 != 0)
                throw new InvalidDataException();

            var unk7 = reader.ReadInt32();

            if (unk7 != 0)
                throw new InvalidDataException();

            return new DataCenterHeader(version, unk1, unk2, unk3, clientVersion, unk4, unk5, unk6, unk7);
        }

        static DataCenterFooter ReadFooter(GameBinaryReader reader)
        {
            var unk1 = reader.ReadInt32();

            if (unk1 != 0)
                throw new InvalidDataException();

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
            var segments = new List<DataCenterSimpleRegion>();

            for (var i = 0; i < count; i++)
                segments.Add(ReadSimpleRegion(reader, false, elementSize));

            return new DataCenterSegmentedSimpleRegion(elementSize, segments);
        }

        static DataCenterRegion ReadRegion(GameBinaryReader reader, uint elementSize)
        {
            var full = reader.ReadUInt32();
            var used = reader.ReadUInt32();

            if (used > full)
                throw new InvalidDataException();

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
