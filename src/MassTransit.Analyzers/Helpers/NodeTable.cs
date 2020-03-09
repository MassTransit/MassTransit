namespace MassTransit.Analyzers.Helpers
{
    using System.Collections.Generic;


    class NodeTable<T>
    {
        readonly IDictionary<T, int> _nodes;
        int _count;

        public NodeTable(int capacity)
        {
            _nodes = new Dictionary<T, int>(capacity);
        }

        /// <summary>
        /// Returns the index for the specified key, which can be any type that supports
        /// equality comparison
        /// </summary>
        /// <param name="key">The key to retrieve</param>
        /// <returns>The index that uniquely relates to the specified key</returns>
        public int this[T key]
        {
            get
            {
                if (_nodes.TryGetValue(key, out var value))
                    return value;

                value = ++_count;
                _nodes.Add(key, value);

                return value;
            }
        }
    }
}
