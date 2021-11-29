namespace MassTransit.Internals.GraphValidation
{
    using System;
    using System.Collections.Generic;


    public class AdjacencyList<T, TNode>
        where TNode : Node<T>
    {
        readonly IDictionary<TNode, HashSet<Edge<T, TNode>>> _edges;
        readonly NodeList<T, TNode> _nodeList;

        public AdjacencyList(Func<int, T, TNode> nodeFactory, int capacity)
        {
            _nodeList = new NodeList<T, TNode>(nodeFactory, capacity);
            _edges = new Dictionary<TNode, HashSet<Edge<T, TNode>>>();
        }

        public IEnumerable<TNode> SourceNodes => _nodeList;

        public HashSet<Edge<T, TNode>> GetEdges(TNode index)
        {
            if (_edges.TryGetValue(index, out HashSet<Edge<T, TNode>> edges))
                return edges;

            return new HashSet<Edge<T, TNode>>();
        }

        public void AddEdge(T source, T target, int weight)
        {
            var sourceNode = _nodeList[source];
            var targetNode = _nodeList[target];

            AddEdge(sourceNode, targetNode, weight);
        }

        void AddEdge(TNode source, TNode target, int weight)
        {
            if (!_edges.TryGetValue(source, out HashSet<Edge<T, TNode>> edges))
            {
                edges = new HashSet<Edge<T, TNode>>();
                _edges.Add(source, edges);
            }

            edges.Add(new Edge<T, TNode>(source, target, weight));
        }

        public TNode GetNode(T key)
        {
            return _nodeList[key];
        }
    }
}
