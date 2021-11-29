namespace MassTransit.Caching
{
    /// <summary>
    /// Observes behavior within the cache
    /// </summary>
    /// <typeparam name="TValue">The value type</typeparam>
    public interface ICacheValueObserver<TValue>
        where TValue : class
    {
        /// <summary>
        /// Called when a new node is added to the cache, after the node has resolved.
        /// </summary>
        /// <param name="node">The cached node</param>
        /// <param name="value">The cached value, to avoid awaiting</param>
        /// <returns>An awaitable task for the observer</returns>
        void ValueAdded(INode<TValue> node, TValue value);

        /// <summary>
        /// Called when a node is removed from the cache.
        /// </summary>
        /// <param name="node">The cached node</param>
        /// <param name="value">The cached value, to avoid awaiting</param>
        /// <returns>An awaitable task for the observer</returns>
        void ValueRemoved(INode<TValue> node, TValue value);

        /// <summary>
        /// Called when the cache is cleared of all nodes.
        /// </summary>
        void CacheCleared();
    }
}
