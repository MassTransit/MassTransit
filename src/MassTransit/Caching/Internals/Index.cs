namespace MassTransit.Caching.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;


    public class Index<TKey, TValue> :
        ICacheIndex<TValue>,
        ICacheValueObserver<TValue>,
        IIndex<TKey, TValue>
        where TValue : class
    {
        readonly KeyProvider<TKey, TValue> _keyProvider;
        readonly object _lock = new object();
        readonly INodeTracker<TValue> _nodeTracker;
        Dictionary<TKey, WeakReference<INode<TValue>>> _index;

        public Index(INodeTracker<TValue> nodeTracker, KeyProvider<TKey, TValue> keyProvider)
        {
            _nodeTracker = nodeTracker;
            _keyProvider = keyProvider;

            _index = new Dictionary<TKey, WeakReference<INode<TValue>>>();

            _nodeTracker.Connect(this);
        }

        Type ICacheIndex<TValue>.KeyType => typeof(TKey);

        public void Clear()
        {
            lock (_lock)
            {
                _index.Clear();
            }
        }

        public async Task<bool> Add(INode<TValue> node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            var value = await node.Value.ConfigureAwait(false);

            var key = _keyProvider(value);

            lock (_lock)
            {
                if (!_index.TryGetValue(key, out WeakReference<INode<TValue>> existingReference)
                    || !existingReference.TryGetTarget(out INode<TValue> existingNode)
                    || existingNode.Value == null)
                {
                    _index[key] = new WeakReference<INode<TValue>>(node, false);
                    return true;
                }

                return false;
            }
        }

        public bool TryGetExistingNode(TValue value, out INode<TValue> node)
        {
            var key = _keyProvider(value);

            lock (_lock)
            {
                return TryGetExistingNode(key, out node);
            }
        }

        public void ValueAdded(INode<TValue> node, TValue value)
        {
            var key = _keyProvider(value);

            lock (_lock)
            {
                _index[key] = new WeakReference<INode<TValue>>(node, false);
            }
        }

        public void ValueRemoved(INode<TValue> node, TValue value)
        {
            var key = _keyProvider(value);

            lock (_lock)
            {
                _index.Remove(key);
            }
        }

        public void CacheCleared()
        {
            Rebuild();
        }

        public Task<TValue> Get(TKey key, MissingValueFactory<TKey, TValue> missingValueFactory)
        {
            lock (_lock)
            {
                if (TryGetExistingNode(key, out INode<TValue> existingNode))
                {
                    if (existingNode.HasValue)
                    {
                        _nodeTracker.Statistics.Hit();
                        return existingNode.Value;
                    }

                    var pending = new PendingValue<TKey, TValue>(key, missingValueFactory);

                    return existingNode.GetValue(pending);
                }

                if (missingValueFactory == null)
                {
                    _nodeTracker.Statistics.Miss();
                    throw new KeyNotFoundException($"Key not found: {key}");
                }

                var pendingValue = new PendingValue<TKey, TValue>(key, missingValueFactory);

                var nodeValueFactory = new NodeValueFactory<TValue>(pendingValue, 0);

                var node = new FactoryNode<TValue>(nodeValueFactory);

                _index[key] = new WeakReference<INode<TValue>>(node, false);

                _nodeTracker.Add(nodeValueFactory);

                return pendingValue.Value;
            }
        }

        public bool Remove(TKey key)
        {
            lock (_lock)
            {
                if (!TryGetExistingNode(key, out INode<TValue> existingNode))
                    return false;

                var result = _index.Remove(key);

                // if the node is resolved, it's likely been notified, so remove it from
                // the rest of the cache
                if (existingNode.HasValue)
                    _nodeTracker.Remove(existingNode);

                return result;
            }
        }

        bool TryGetExistingNode(TKey key, out INode<TValue> existingNode)
        {
            if (_index.TryGetValue(key, out WeakReference<INode<TValue>> existingReference))
            {
                if (existingReference.TryGetTarget(out existingNode) && existingNode.IsValid)
                    return true;

                _index.Remove(key);
            }

            existingNode = null;
            return false;
        }

        void Rebuild()
        {
            lock (_lock)
            {
                // this will throw an exception if there is a duplicate key, but until we support multi-value indices, that's okay
                Dictionary<TKey, WeakReference<INode<TValue>>> updatedIndex = _nodeTracker.GetAll()
                    .ToDictionary(node => _keyProvider(node.Value.Result), node => new WeakReference<INode<TValue>>(node, false));

                _index = updatedIndex;
            }
        }
    }
}
