namespace MassTransit.Util.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;


    public class GreenCache<TValue> :
        ICache<TValue>
        where TValue : class
    {
        readonly IDictionary<string, ICacheIndex<TValue>> _indices;
        readonly INodeTracker<TValue> _nodeTracker;

        /// <summary>
        ///
        /// </summary>
        /// <param name="capacity">The typical maximum capacity of the cache (not a hard limit)</param>
        /// <param name="minAge">The minmum time an item is cached before being eligible for removal</param>
        /// <param name="maxAge">The maximum time an unaccessed item will remain in the cache</param>
        /// <param name="nowProvider">Provides the current time</param>
        public GreenCache(int capacity, TimeSpan minAge, TimeSpan maxAge, CurrentTimeProvider nowProvider)
        {
            _indices = new Dictionary<string, ICacheIndex<TValue>>();

            _nodeTracker = new NodeTracker<TValue>(capacity, minAge, maxAge, nowProvider);
        }

        public CacheStatistics Statistics => _nodeTracker.Statistics;

        public IIndex<TKey, TValue> AddIndex<TKey>(string indexName, KeyProvider<TKey, TValue> keyProvider,
            MissingValueFactory<TKey, TValue> missingValueFactory = null)
        {
            if (_indices.ContainsKey(indexName))
                throw new ArgumentException($"An index with the same name was already added: {indexName}", nameof(indexName));

            var index = new Index<TKey, TValue>(_nodeTracker, keyProvider);

            _indices[indexName] = index;

            return index;
        }

        public IIndex<TKey, TValue> GetIndex<TKey>(string indexName)
        {
            ICacheIndex<TValue> index;
            if (_indices.TryGetValue(indexName, out index) && index is IIndex<TKey, TValue>)
                return (IIndex<TKey, TValue>)index;

            throw new ArgumentException($"An index named {indexName} was not found", nameof(indexName));
        }

        public void Add(TValue value)
        {
            _nodeTracker.Add(value);
        }

        public void Clear()
        {
            _nodeTracker.Clear();
        }

        public IEnumerable<INode<TValue>> GetAll()
        {
            return _nodeTracker.GetAll();
        }
    }
}