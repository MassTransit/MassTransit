using System.Collections.Generic;

namespace MassTransit.Analyzers.Helpers
{
    class NodeList<T>
    {
        readonly NodeTable<T> _nodeTable;
        readonly IList<HashSet<int>> _nodes;

        public NodeList(int capacity)
        {
            _nodes = new List<HashSet<int>>(capacity);
            _nodeTable = new NodeTable<T>(capacity);
        }

        public void Add(T key, params T[] values)
        {
            var hashSet = _nodes[Index(key) - 1];
            for (int i = 0; i < values.Length; i++)
            {
                hashSet.Add(Index(values[i]));
            }
        }

        public bool Contains(T key, T value)
        {
            return _nodes[Index(key) - 1].Contains(Index(value));
        }

        /// <summary>
        /// Retrieve the index for a given key
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>The index</returns>
        int Index(T key)
        {
            int index = _nodeTable[key];

            if (index <= _nodes.Count)
                return index;

            _nodes.Add(new HashSet<int>());
            return index;
        }
    }
}
