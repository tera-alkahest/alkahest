using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Alkahest.Core.Data
{
    public sealed class DataCenterElement : IEnumerable<DataCenterElement>
    {
        public DataCenterElement Parent { get; private set; }

        public string Name { get; }

        public IReadOnlyList<DataCenterAttribute> Attributes => _attributes.Value;

        public DataCenterAttribute this[string name] => Attribute(name);

        readonly Lazy<IReadOnlyList<DataCenterAttribute>> _attributes;

        readonly Lazy<IReadOnlyList<DataCenterElement>> _children;

        internal DataCenterElement(DataCenter center,
            DataCenterAddress address)
        {
            ushort attrCount;
            ushort childCount;
            DataCenterAddress attrAddr;
            DataCenterAddress childAddr;

            try
            {
                center.Lock.EnterReadLock();

                if (center.Disposed)
                    throw new ObjectDisposedException(center.GetType().FullName);

                var reader = center.Elements.GetReader(address);
                var nameIndex = reader.ReadUInt16();

                if (nameIndex >= center.Names.Count)
                    throw new InvalidDataException();

                Name = center.Names[nameIndex];

                reader.ReadUInt16();

                attrCount = reader.ReadUInt16();
                childCount = reader.ReadUInt16();
                attrAddr = DataCenter.ReadAddress(reader);
                childAddr = DataCenter.ReadAddress(reader);
            }
            finally
            {
                center.Lock.ExitReadLock();
            }

            _attributes = new Lazy<IReadOnlyList<DataCenterAttribute>>(() =>
            {
                var attributes = new List<DataCenterAttribute>();

                try
                {
                    center.Lock.EnterReadLock();

                    if (center.Disposed)
                        throw new ObjectDisposedException(center.GetType().FullName);

                    for (var i = 0; i < attrCount; i++)
                    {
                        var addr = new DataCenterAddress(attrAddr.SegmentIndex,
                            (ushort)(attrAddr.ElementIndex + i));
                        var attrReader = center.Attributes.GetReader(addr);

                        var attrNameIndex = attrReader.ReadUInt16();

                        if (attrNameIndex >= center.Names.Count)
                            throw new InvalidDataException();

                        var typeCode = (DataCenterTypeCode)attrReader.ReadUInt16();

                        if (typeCode != DataCenterTypeCode.Int32 &&
                            typeCode != DataCenterTypeCode.Single &&
                            typeCode != DataCenterTypeCode.Boolean)
                            typeCode = DataCenterTypeCode.String;

                        var primitiveValue = attrReader.ReadInt32();
                        string stringValue = null;

                        if (typeCode == DataCenterTypeCode.String)
                        {
                            attrReader.Position -= sizeof(int);

                            var strAddr = DataCenter.ReadAddress(attrReader);

                            stringValue = center.GetString(strAddr);
                        }

                        attributes.Add(new DataCenterAttribute(
                            center.Names[attrNameIndex], typeCode,
                            primitiveValue, stringValue));
                    }
                }
                finally
                {
                    center.Lock.ExitReadLock();
                }

                return attributes;
            });

            _children = new Lazy<IReadOnlyList<DataCenterElement>>(() =>
            {
                var children = new List<DataCenterElement>();

                for (var i = 0; i < childCount; i++)
                {
                    var addr = new DataCenterAddress(childAddr.SegmentIndex,
                        (ushort)(childAddr.ElementIndex + i));

                    children.Add(new DataCenterElement(center, addr)
                    {
                        Parent = this
                    });
                }

                return children;
            });
        }

        public IEnumerator<DataCenterElement> GetEnumerator()
        {
            foreach (var child in _children.Value)
                yield return child;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public DataCenterAttribute Attribute(string name)
        {
            return Attributes.SingleOrDefault(x => x.Name == name);
        }

        public DataCenterElement Child(string name)
        {
            return this.SingleOrDefault(x => x.Name == name);
        }

        public IEnumerable<DataCenterElement> Children(string name)
        {
            return this.Where(x => x.Name == name);
        }

        public override string ToString()
        {
            return $"[Name: {Name}, Parent: {Parent?.Name ?? "N/A"}]";
        }
    }
}
