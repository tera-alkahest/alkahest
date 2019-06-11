using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Alkahest.Core.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public sealed class DataCenterElement : IEnumerable<DataCenterElement>, IDisposable
    {
        object _parent;

        public string Name { get; }

        Lazy<IReadOnlyDictionary<string, DataCenterValue>> _attributes;

        Lazy<IReadOnlyList<DataCenterElement>> _children;

        public DataCenter Center => _parent is DataCenterElement e ? e.Center : (DataCenter)_parent;

        public DataCenterElement Parent => _parent is DataCenter ? null : (DataCenterElement)_parent;

        public IReadOnlyDictionary<string, DataCenterValue> Attributes
        {
            get
            {
                if (_attributes == null)
                    throw new ObjectDisposedException(GetType().FullName);

                return _attributes.Value;
            }
        }

        public DataCenterValue this[string name] => Attribute(name);

        public DataCenterValue this[string name, object fallback] => AttributeOrDefault(name, fallback);

        internal DataCenterElement(DataCenter center, DataCenterAddress address)
        {
            if (address == DataCenterAddress.Zero)
            {
                // Are we creating a dummy root element?
                if (center.Names == null)
                {
                    Name = "__root__";
                    _attributes = new Lazy<IReadOnlyDictionary<string, DataCenterValue>>(
                        () => new Dictionary<string, DataCenterValue>());
                    _children = new Lazy<IReadOnlyList<DataCenterElement>>(
                        () => new List<DataCenterElement>());

                    return;
                }

                _parent = center;
            }

            ushort attrCount;
            ushort childCount;
            DataCenterAddress attrAddr;
            DataCenterAddress childAddr;

            try
            {
                center.Lock.EnterReadLock();

                if (center.IsDisposed)
                    throw new ObjectDisposedException(center.GetType().FullName);

                var reader = center.Elements.GetReader(address);
                var nameIndex = reader.ReadUInt16() - 1;

                if (nameIndex == -1)
                    throw new DataCenterPlaceholderException();

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

            _attributes = new Lazy<IReadOnlyDictionary<string, DataCenterValue>>(() =>
            {
                var attributes = new Dictionary<string, DataCenterValue>();

                try
                {
                    center.Lock.EnterReadLock();

                    if (center.IsDisposed)
                        throw new ObjectDisposedException(center.GetType().FullName);

                    for (var i = 0; i < attrCount; i++)
                    {
                        var addr = new DataCenterAddress(attrAddr.SegmentIndex,
                            (ushort)(attrAddr.ElementIndex + i));
                        var attrReader = center.Attributes.GetReader(addr);
                        var attrNameIndex = attrReader.ReadUInt16() - 1;

                        if (attrNameIndex >= center.Names.Count)
                            throw new InvalidDataException();

                        var typeCode = (DataCenterTypeCode)attrReader.ReadUInt16();

                        switch (typeCode)
                        {
                            case DataCenterTypeCode.Int32:
                            case DataCenterTypeCode.Single:
                            case DataCenterTypeCode.Boolean:
                                break;
                            default:
                                typeCode = DataCenterTypeCode.String;
                                break;
                        }

                        var primitiveValue = attrReader.ReadInt32();
                        string stringValue = null;

                        if (typeCode == DataCenterTypeCode.String)
                        {
                            attrReader.Position -= sizeof(int);

                            var strAddr = DataCenter.ReadAddress(attrReader);

                            stringValue = center.GetString(strAddr);
                        }

                        attributes.Add(center.Names[attrNameIndex], new DataCenterValue(typeCode,
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

                    try
                    {
                        children.Add(new DataCenterElement(center, addr)
                        {
                            _parent = this,
                        });
                    }
                    catch (DataCenterPlaceholderException)
                    {
                    }
                }

                return children;
            });
        }

        public void Dispose()
        {
            if (Center.IsFrozen)
                throw new InvalidOperationException("Data center is frozen.");

            _attributes = null;
            _children = null;
        }

        public IEnumerator<DataCenterElement> GetEnumerator()
        {
            if (_children == null)
                throw new ObjectDisposedException(GetType().FullName);

            return _children.Value.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public DataCenterValue Attribute(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            return Attributes.GetValueOrDefault(name);
        }

        public DataCenterValue AttributeOrDefault(string name, object fallback)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (!(fallback is int || fallback is float || fallback is string || fallback is bool))
                throw new ArgumentException("Invalid fallback value.", nameof(fallback));

            var attr = Attributes.GetValueOrDefault(name);

            if (!attr.IsNull)
                return attr;

            switch (fallback)
            {
                case int i:
                    return new DataCenterValue(DataCenterTypeCode.Int32, i, null);
                case float f:
                    return new DataCenterValue(DataCenterTypeCode.Single, Unsafe.As<float, int>(ref f), null);
                case string s:
                    return new DataCenterValue(DataCenterTypeCode.String, 0, s);
                case bool b:
                    return new DataCenterValue(DataCenterTypeCode.Boolean, b ? 1 : 0, null);
                default:
                    throw Assert.Unreachable();
            }
        }

        public DataCenterElement Child(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            return this.SingleOrDefault(x => x.Name == name);
        }

        public DataCenterElement FirstChild(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            return this.FirstOrDefault(x => x.Name == name);
        }

        public DataCenterElement LastChild(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            return this.LastOrDefault(x => x.Name == name);
        }

        public IEnumerable<DataCenterElement> Children(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            return this.Where(x => x.Name == name);
        }

        public override string ToString()
        {
            return $"[Name: {Name}, Parent: {Parent?.Name ?? "N/A"}]";
        }
    }
}
