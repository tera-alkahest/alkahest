using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Alkahest.Core.Collections
{
    public class NoNullList<T> : IList<T>, IReadOnlyList<T>, IList
        where T : class
    {
        readonly List<T> _list;

        public int Count => _list.Count;

        bool IList.IsFixedSize => ((IList)_list).IsFixedSize;

        bool ICollection<T>.IsReadOnly => ((ICollection<T>)_list).IsReadOnly;

        bool IList.IsReadOnly => ((IList)_list).IsReadOnly;

        bool ICollection.IsSynchronized => ((ICollection)_list).IsSynchronized;

        object ICollection.SyncRoot => ((ICollection)_list).SyncRoot;

        public T this[int index]
        {
            get => _list[index];
            set => _list[index] = value ?? throw new ArgumentNullException(nameof(value));
        }

        object IList.this[int index]
        {
            get => ((IList)_list)[index];
            set => ((IList)_list)[index] = value ?? throw new ArgumentNullException(nameof(value));
        }

        public NoNullList()
        {
            _list = new List<T>();
        }

        public NoNullList(int capacity)
        {
            _list = new List<T>(capacity);
        }

        public NoNullList(IEnumerable<T> collection)
        {
            _list = new List<T>(collection);

            if (collection.Any(x => x == null))
                throw new ArgumentException("Null item given.", nameof(collection));
        }

        public void Add(T item)
        {
            _list.Add(item ?? throw new ArgumentNullException(nameof(item)));
        }

        int IList.Add(object value)
        {
            return ((IList)_list).Add(value ?? throw new ArgumentNullException(nameof(value)));
        }

        public void AddRange(IEnumerable<T> collection)
        {
            if (collection.Any(x => x == null))
                throw new ArgumentException("Null item given.", nameof(collection));

            _list.AddRange(collection);
        }

        public ReadOnlyCollection<T> AsReadOnly()
        {
            return _list.AsReadOnly();
        }

        public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
        {
            return _list.BinarySearch(index, count,
                item ?? throw new ArgumentNullException(nameof(item)), comparer);
        }

        public int BinarySearch(T item)
        {
            return _list.BinarySearch(item ?? throw new ArgumentNullException(nameof(item)));
        }

        public int BinarySearch(T item, IComparer<T> comparer)
        {
            return _list.BinarySearch(item ?? throw new ArgumentNullException(nameof(item)), comparer);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(T item)
        {
            return _list.Contains(item ?? throw new ArgumentNullException(nameof(item)));
        }

        bool IList.Contains(object value)
        {
            return ((IList)_list).Contains(value ?? throw new ArgumentNullException(nameof(value)));
        }

        public void CopyTo(int index, T[] array, int arrayIndex, int count)
        {
            _list.CopyTo(index, array, arrayIndex, count);
        }

        public void CopyTo(T[] array)
        {
            _list.CopyTo(array);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((IList)_list).CopyTo(array, index);
        }

        public int FindIndex(int startIndex, int count, Predicate<T> match)
        {
            return _list.FindIndex(startIndex, count, match);
        }

        public int FindIndex(int startIndex, Predicate<T> match)
        {
            return _list.FindIndex(startIndex, match);
        }

        public int FindIndex(Predicate<T> match)
        {
            return _list.FindIndex(match);
        }

        public int FindLastIndex(int startIndex, int count, Predicate<T> match)
        {
            return _list.FindLastIndex(startIndex, count, match);
        }

        public int FindLastIndex(int startIndex, Predicate<T> match)
        {
            return _list.FindLastIndex(startIndex, match);
        }

        public int FindLastIndex(Predicate<T> match)
        {
            return _list.FindLastIndex(match);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_list).GetEnumerator();
        }

        public NoNullList<T> GetRange(int index, int count)
        {
            return new NoNullList<T>(_list.GetRange(index, count));
        }

        public int IndexOf(T item)
        {
            return _list.IndexOf(item ?? throw new ArgumentNullException(nameof(item)));
        }

        public int IndexOf(T item, int index)
        {
            return _list.IndexOf(item ?? throw new ArgumentNullException(nameof(item)), index);
        }

        public int IndexOf(T item, int index, int count)
        {
            return _list.IndexOf(item ?? throw new ArgumentNullException(nameof(item)), index, count);
        }

        int IList.IndexOf(object value)
        {
            return ((IList)_list).IndexOf(value ?? throw new ArgumentNullException(nameof(value)));
        }

        public void Insert(int index, T item)
        {
            _list.Insert(index, item ?? throw new ArgumentNullException(nameof(item)));
        }

        void IList.Insert(int index, object value)
        {
            ((IList)_list).Insert(index, value ?? throw new ArgumentNullException(nameof(value)));
        }

        public void InsertRange(int index, IEnumerable<T> collection)
        {
            if (collection.Any(x => x == null))
                throw new ArgumentException("Null item given.", nameof(collection));

            _list.InsertRange(index, collection);
        }

        public int LastIndexOf(T item)
        {
            return _list.LastIndexOf(item ?? throw new ArgumentNullException(nameof(item)));
        }

        public int LastIndexOf(T item, int count)
        {
            return _list.LastIndexOf(item ?? throw new ArgumentNullException(nameof(item)), count);
        }

        public int LastIndexOf(T item, int count, int index)
        {
            return _list.LastIndexOf(item ?? throw new ArgumentNullException(nameof(item)), count, index);
        }

        public bool Remove(T item)
        {
            return _list.Remove(item ?? throw new ArgumentNullException(nameof(item)));
        }

        void IList.Remove(object value)
        {
            ((IList)_list).Remove(value ?? throw new ArgumentNullException(nameof(value)));
        }

        public int RemoveAll(Predicate<T> match)
        {
            return _list.RemoveAll(match);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        public void RemoveRange(int index, int count)
        {
            _list.RemoveRange(index, count);
        }

        public void Reverse()
        {
            _list.Reverse();
        }

        public void Reverse(int index, int count)
        {
            _list.Reverse(index, count);
        }

        public T[] ToArray()
        {
            return _list.ToArray();
        }

        public void TrimExcess()
        {
            _list.TrimExcess();
        }
    }
}
