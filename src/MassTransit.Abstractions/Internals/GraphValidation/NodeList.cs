namespace MassTransit.Internals.GraphValidation
{
    using System;
    using System.Collections;
    using System.Collections.Generic;


    /// <summary>
    /// Maintains a list of nodes for a given set of instances of T
    /// </summary>
    /// <typeparam name="T">The type encapsulated in the node</typeparam>
    /// <typeparam name="TNode">The type of node contained in the list</typeparam>
    public class NodeList<T, TNode> :
        IEnumerable<TNode>
        where TNode : Node<T>
    {
        readonly Func<int, T, TNode> _nodeFactory;
        readonly IList<TNode> _nodes;
        readonly NodeTable<T> _nodeTable;

        public NodeList(Func<int, T, TNode> nodeFactory, int capacity)
        {
            _nodeFactory = nodeFactory;
            _nodes = new List<TNode>(capacity);
            _nodeTable = new NodeTable<T>(capacity);
        }

        /// <summary>
        /// Retrieves the node for the given key
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>The unique node that relates to the specified key</returns>
        public TNode this[T key] => _nodes[Index(key) - 1];

        public IEnumerator<TNode> GetEnumerator()
        {
            return _nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Retrieve the index for a given key
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>The index</returns>
        public int Index(T key)
        {
            var index = _nodeTable[key];

            if (index <= _nodes.Count)
                return index;

            var node = _nodeFactory(index, key);
            _nodes.Add(node);

            return index;
        }
    }
}
