namespace MassTransit.Analyzers.Helpers
{
    using System.Collections.Concurrent;


    class NodeTable<T>
    {
        readonly ConcurrentDictionary<T, int> _nodes;
        int _count;

        public NodeTable(int capacity)
        {
            _nodes = new ConcurrentDictionary<T, int>(8, capacity);
        }

        /// <summary>
        /// Returns the index for the specified key, which can be any type that supports
        /// equality comparison
        /// </summary>
        /// <param name="key">The key to retrieve</param>
        /// <returns>The index that uniquely relates to the specified key</returns>
        public int this[T key]
        {
            get { return _nodes.GetOrAdd(key, k => ++_count); }
        }
    }
}
