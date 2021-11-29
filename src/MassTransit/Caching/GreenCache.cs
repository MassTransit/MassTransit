namespace MassTransit.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Internals;


    public class GreenCache<TValue> :
        ICache<TValue>
        where TValue : class
    {
        readonly IDictionary<string, ICacheIndex<TValue>> _indices;
        readonly INodeTracker<TValue> _nodeTracker;

        /// <summary>
        /// Create a cache using the specified cache settings
        /// </summary>
        /// <param name="settings">The cache settings</param>
        public GreenCache(CacheSettings settings = null)
        {
            _indices = new Dictionary<string, ICacheIndex<TValue>>();

            _nodeTracker = new NodeTracker<TValue>(settings ?? new CacheSettings());
        }

        public CacheStatistics Statistics => _nodeTracker.Statistics;

        public IIndex<TKey, TValue> AddIndex<TKey>(string name, KeyProvider<TKey, TValue> keyProvider,
            MissingValueFactory<TKey, TValue> missingValueFactory = null)
        {
            lock (_indices)
            {
                if (_indices.ContainsKey(name))
                    throw new ArgumentException($"An index with the same name was already added: {name}", nameof(name));

                var index = new Index<TKey, TValue>(_nodeTracker, keyProvider);

                _indices[name] = index;

                return index;
            }
        }

        public IIndex<TKey, TValue> GetIndex<TKey>(string name)
        {
            lock (_indices)
            {
                if (!_indices.TryGetValue(name, out ICacheIndex<TValue> cacheIndex))
                    throw new ArgumentException($"An index named {name} was not found", nameof(name));

                if (cacheIndex is IIndex<TKey, TValue> index)
                    return index;

                throw new ArgumentException(
                    $"The index named {name} key type {TypeCache.GetShortName(cacheIndex.KeyType)} did not match the specified key type {TypeCache<TKey>.ShortName}",
                    nameof(TKey));
            }
        }

        public void Add(TValue value)
        {
            _nodeTracker.Add(value);
        }

        public void Clear()
        {
            _nodeTracker.Clear();
        }

        public IEnumerable<Task<TValue>> GetAll()
        {
            return _nodeTracker.GetAll().Select(x => x.Value);
        }

        public ConnectHandle Connect(ICacheValueObserver<TValue> observer)
        {
            return _nodeTracker.Connect(observer);
        }
    }
}
