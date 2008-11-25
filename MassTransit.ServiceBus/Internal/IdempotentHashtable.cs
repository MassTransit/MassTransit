namespace MassTransit.Internal
{
    using System.Collections;
    using System.Collections.Generic;

    public class IdempotentHashtable<K, V> :
        IEnumerable<KeyValuePair<K, V>>
    {
        readonly IDictionary<K,V> _cache = new Dictionary<K, V>();

        public void Add(K key, V value)
        {
            if(!_cache.ContainsKey(key))
            {
                _cache.Add(key, value);
            }
        }

        public void Remove(K key)
        {
            if(_cache.ContainsKey(key))
            {
                _cache.Remove(key);
            }
        }

        public V this[K key]
        {
            get
            {
                return _cache[key];
            }
            set
            {
                Add(key, value);
            }
        }

        public int Count
        {
            get { return _cache.Count; }
        }

        #region Implementation of IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Implementation of IEnumerable<KeyValuePair<K,V>>

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            return _cache.GetEnumerator();
        }

        #endregion
    }
}