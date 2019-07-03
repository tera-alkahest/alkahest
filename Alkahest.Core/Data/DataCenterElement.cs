using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Alkahest.Core.Data
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public sealed class DataCenterElement
    {
        object _parent;

        public string Name { get; }

        readonly Lazy<IReadOnlyDictionary<string, DataCenterValue>> _attributes;

        readonly Lazy<IReadOnlyList<DataCenterElement>> _children;

        public DataCenter Center => _parent is DataCenterElement e ? e.Center : (DataCenter)_parent;

        public DataCenterElement Parent => _parent is DataCenter ? null : (DataCenterElement)_parent;

        public IReadOnlyDictionary<string, DataCenterValue> Attributes => _attributes.Value;

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

            var reader = center.Elements.GetReader(address);
            var nameIndex = reader.ReadUInt16() - 1;
            var placeholder = nameIndex == -1;

            // Is this a placeholder element?
            if (!placeholder)
                Name = center.Names.Get(nameIndex);

            var ext = reader.ReadUInt16();
            var flags = Bits.Extract(ext, 0, 4);

            if (flags != 0)
                throw new InvalidDataException($"Unexpected extension flags {flags}.");

            var extIndex = Bits.Extract(ext, 4, 12);

            if (placeholder && extIndex != 0)
                throw new InvalidDataException(
                    $"Placeholder element has non-zero extension index {extIndex}.");
            else if (extIndex >= center.Extensions.Count)
                throw new InvalidDataException(
                    $"Extension index {extIndex} is greater than {center.Extensions.Count}.");

            var attrCount = reader.ReadUInt16();

            if (placeholder && attrCount != 0)
                throw new InvalidDataException(
                    $"Placeholder element has non-zero attribute count {attrCount}.");

            var childCount = reader.ReadUInt16();

            if (placeholder && childCount != 0)
                throw new InvalidDataException(
                    $"Placeholder element has non-zero child count {childCount}.");

            var attrAddr = DataCenter.ReadAddress(reader);

            if (placeholder && attrAddr != DataCenterAddress.Invalid)
                throw new InvalidDataException(
                    $"Placeholder element has unexpected attribute address {attrAddr}.");
            else if (attrCount == 0 && attrAddr != DataCenterAddress.Invalid)
                throw new InvalidDataException(
                    $"Element with zero attributes has unexpected attribute address {attrAddr}.");
            else if (attrCount != 0 && attrAddr == DataCenterAddress.Invalid)
                throw new InvalidDataException(
                    $"Element with {attrCount} attributes has invalid attribute address {attrAddr}.");

            var childAddr = DataCenter.ReadAddress(reader);

            if (placeholder && childAddr != DataCenterAddress.Invalid)
                throw new InvalidDataException(
                    $"Placeholder element has unexpected child address {childAddr}.");
            else if (childCount == 0 && childAddr != DataCenterAddress.Invalid)
                throw new InvalidDataException(
                    $"Element with zero children has unexpected child address {childAddr}.");
            else if (childCount != 0 && childAddr == DataCenterAddress.Invalid)
                throw new InvalidDataException(
                    $"Element with {childCount} children has invalid child address {childAddr}.");

            if (placeholder)
                return;

            _attributes = new Lazy<IReadOnlyDictionary<string, DataCenterValue>>(() =>
            {
                var attributes = new Dictionary<string, DataCenterValue>();

                for (var i = 0; i < attrCount; i++)
                {
                    var addr = new DataCenterAddress(attrAddr.SegmentIndex,
                        (ushort)(attrAddr.ElementIndex + i));
                    var attrReader = center.Attributes.GetReader(addr);
                    var name = center.Names.Get(attrReader.ReadUInt16() - 1);
                    var typeInfo = attrReader.ReadUInt16();
                    var typeCode = Bits.Extract(typeInfo, 0, 2);
                    var extCode = Bits.Extract(typeInfo, 2, 14);

                    DataCenterTypeCode type;

                    switch (typeCode)
                    {
                        case 1:
                            switch (extCode)
                            {
                                case 0:
                                    type = DataCenterTypeCode.Int32;
                                    break;
                                case 1:
                                    type = DataCenterTypeCode.Boolean;
                                    break;
                                default:
                                    throw new InvalidDataException(
                                        $"Unexpected extended code {extCode} for integer value.");
                            }

                            break;
                        case 2:
                            if (extCode != 0)
                                throw new InvalidDataException(
                                    $"Unexpected extended code {extCode} for floating point value.");

                            type = DataCenterTypeCode.Single;
                            break;
                        case 3:
                            type = DataCenterTypeCode.String;
                            break;
                        default:
                            throw new InvalidDataException($"Unexpected type code value {typeCode}.");
                    }

                    var primitiveValue = attrReader.ReadInt32();
                    string stringValue = null;

                    switch (type)
                    {
                        case DataCenterTypeCode.String:
                            attrReader.Position -= sizeof(int);

                            stringValue = center.Values.Get(DataCenter.ReadAddress(attrReader));
                            break;
                        case DataCenterTypeCode.Boolean:
                            if (primitiveValue != 0 && primitiveValue != 1)
                                throw new InvalidDataException(
                                    $"Unexpected non-Boolean value {primitiveValue}.");
                            break;
                    }

                    if (!attributes.TryAdd(name, new DataCenterValue(type, primitiveValue, stringValue)))
                        throw new InvalidDataException($"Duplicate attribute name {name}.");
                }

                return attributes;
            });

            _children = new Lazy<IReadOnlyList<DataCenterElement>>(() =>
            {
                var children = new List<DataCenterElement>();

                for (var i = 0; i < childCount; i++)
                {
                    var elem = center.Materialize(new DataCenterAddress(
                        childAddr.SegmentIndex, (ushort)(childAddr.ElementIndex + i)));

                    elem._parent = this;

                    // Don't expose placeholder elements.
                    if (elem.Name != null)
                        children.Add(elem);
                }

                return children;
            });
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

        public IEnumerable<DataCenterElement> Ancestors()
        {
            var current = this;

            while ((current = current.Parent) != null)
                yield return current;
        }

        public IEnumerable<DataCenterElement> Ancestors(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            return Ancestors().Where(x => x.Name == name);
        }

        public IEnumerable<DataCenterElement> Ancestors(params string[] names)
        {
            if (names == null)
                throw new ArgumentNullException(nameof(names));

            if (names.Any(x => x == null))
                throw new ArgumentException("A null name was given.", nameof(names));

            var set = names.ToHashSet();

            return Ancestors().Where(x => set.Contains(x.Name));
        }

        public IEnumerable<DataCenterElement> Siblings()
        {
            var parent = Parent;

            if (parent == null)
                yield break;

            foreach (var elem in parent.Children().Where(x => x != this))
                yield return elem;
        }

        public IEnumerable<DataCenterElement> Siblings(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            return Siblings().Where(x => x.Name == name);
        }

        public IEnumerable<DataCenterElement> Siblings(params string[] names)
        {
            if (names == null)
                throw new ArgumentNullException(nameof(names));

            if (names.Any(x => x == null))
                throw new ArgumentException("A null name was given.", nameof(names));

            var set = names.ToHashSet();

            return Siblings().Where(x => set.Contains(x.Name));
        }

        public IEnumerable<DataCenterElement> Children()
        {
            return _children.Value;
        }

        public IEnumerable<DataCenterElement> Children(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            return Children().Where(x => x.Name == name);
        }

        public IEnumerable<DataCenterElement> Children(params string[] names)
        {
            if (names == null)
                throw new ArgumentNullException(nameof(names));

            if (names.Any(x => x == null))
                throw new ArgumentException("A null name was given.", nameof(names));

            var set = names.ToHashSet();

            return Children().Where(x => set.Contains(x.Name));
        }

        public IEnumerable<DataCenterElement> Descendants()
        {
            var work = new Queue<DataCenterElement>();

            work.Enqueue(this);

            while (work.Count != 0)
            {
                var current = work.Dequeue();

                foreach (var elem in current.Children())
                {
                    yield return elem;

                    work.Enqueue(elem);
                }
            }
        }

        public IEnumerable<DataCenterElement> Descendants(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            return Descendants().Where(x => x.Name == name);
        }

        public IEnumerable<DataCenterElement> Descendants(params string[] names)
        {
            if (names == null)
                throw new ArgumentNullException(nameof(names));

            if (names.Any(x => x == null))
                throw new ArgumentException("A null name was given.", nameof(names));

            var set = names.ToHashSet();

            return Descendants().Where(x => set.Contains(x.Name));
        }

        public override string ToString()
        {
            return $"[Name: {Name}, Parent: {Parent?.Name ?? "N/A"}]";
        }
    }
}
