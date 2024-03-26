namespace MassTransit.Util
{
    using System;
    using System.Collections;
    using System.Collections.Generic;


    public class SingleThreadedDictionary<TKey, TValue> :
        IReadOnlyDictionary<TKey, TValue>
    {
        readonly IEqualityComparer<TKey> _comparer;
        readonly object _lock;
        IDictionary<TKey, TValue> _dictionary;

        public SingleThreadedDictionary(IEqualityComparer<TKey> comparer = default)
        {
            _comparer = comparer ?? EqualityComparer<TKey>.Default;

            _lock = new object();

            _dictionary = new Dictionary<TKey, TValue>(_comparer);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _dictionary.Count;

        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public TValue this[TKey key] => _dictionary[key];

        public IEnumerable<TKey> Keys => _dictionary.Keys;

        public IEnumerable<TValue> Values => _dictionary.Values;

        public void Clear()
        {
            if (_dictionary.Count == 0)
                return;

            lock (_lock)
                _dictionary = new Dictionary<TKey, TValue>(_comparer);
        }

        public bool TryAdd(TKey key, Func<TKey, TValue> valueFactory)
        {
            lock (_lock)
            {
                if (_dictionary.ContainsKey(key))
                    return false;

                var value = valueFactory(key);
                _dictionary = new Dictionary<TKey, TValue>(_dictionary, _comparer) { [key] = value };

                return true;
            }
        }

        public bool TryRemove(TKey key, out TValue value)
        {
            lock (_lock)
            {
                if (!_dictionary.TryGetValue(key, out value))
                    return false;

                var replacement = new Dictionary<TKey, TValue>(_dictionary, _comparer);

                var result = replacement.Remove(key);

                _dictionary = replacement;

                return result;
            }
        }
    }
}
