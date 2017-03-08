using System;
using System.Collections.Concurrent;

namespace Alkahest.Core
{
    public sealed class ObjectPool<T>
    {
        readonly Func<T> _creator;

        readonly Action<T> _cleaner;

        readonly int? _limit;

        readonly ConcurrentBag<T> _bag = new ConcurrentBag<T>();

        public ObjectPool(Func<T> creator, Action<T> cleaner, int? limit)
        {
            _creator = creator;
            _cleaner = cleaner;
            _limit = limit;
        }

        public T Get()
        {
            T item;

            if (_bag.Count >= _limit || !_bag.TryTake(out item))
                item = _creator();

            _cleaner?.Invoke(item);

            return item;
        }

        public bool TryPut(T item)
        {
            if (_bag.Count >= _limit)
                return false;

            _bag.Add(item);
            return true;
        }
    }
}
