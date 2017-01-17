namespace MassTransit.Transports.InMemory.GraphValidation
{
    using System;
    using System.Collections.Generic;


    class AdjacencyList<T, TNode>
        where TNode : Node<T>
    {
        readonly NodeList<T, TNode> _nodeList;
        readonly IDictionary<TNode, HashSet<Edge<T, TNode>>> _adjacencies;


        public AdjacencyList(Func<int, T, TNode> nodeFactory, int capacity)
        {
            _nodeList = new NodeList<T, TNode>(nodeFactory, capacity);
            _adjacencies = new Dictionary<TNode, HashSet<Edge<T, TNode>>>();
        }

        public ICollection<TNode> SourceNodes
        {
            get { return _adjacencies.Keys; }
        }

        public HashSet<Edge<T, TNode>> GetEdges(TNode index)
        {
            HashSet<Edge<T, TNode>> edges;
            if (_adjacencies.TryGetValue(index, out edges))
                return edges;

            return new HashSet<Edge<T, TNode>>();
        }

        public void AddEdge(T source, T target, int weight)
        {
            TNode sourceNode = _nodeList[source];
            TNode targetNode = _nodeList[target];

            AddEdge(sourceNode, targetNode, weight);
        }

        void AddEdge(TNode source, TNode target, int weight)
        {
            HashSet<Edge<T, TNode>> edges;
            if (!_adjacencies.TryGetValue(source, out edges))
            {
                edges = new HashSet<Edge<T, TNode>>();
                _adjacencies.Add(source, edges);
            }

            edges.Add(new Edge<T, TNode>(source, target, weight));
        }

        public TNode GetNode(T key)
        {
            return _nodeList[key];
        }
    }
}