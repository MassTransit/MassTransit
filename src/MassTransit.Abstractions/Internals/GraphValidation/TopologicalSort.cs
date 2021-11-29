namespace MassTransit.Internals.GraphValidation
{
    using System.Collections.Generic;
    using System.Linq;


    public class TopologicalSort<T, TNode>
        where TNode : Node<T>, ITopologicalSortNodeProperties
    {
        readonly AdjacencyList<T, TNode> _list;
        readonly IList<TNode> _results;
        readonly IEnumerable<TNode> _sourceNodes;

        public TopologicalSort(AdjacencyList<T, TNode> list)
        {
            _list = list;
            _results = new List<TNode>();
            _sourceNodes = _list.SourceNodes;

            Sort();
        }

        public TopologicalSort(AdjacencyList<T, TNode> list, T source)
        {
            _list = list;
            _results = new List<TNode>();

            var sourceNode = list.GetNode(source);
            _sourceNodes = Enumerable.Repeat(sourceNode, 1);

            Sort();
        }

        public IEnumerable<TNode> Result => _results;

        void Sort()
        {
            foreach (var node in _sourceNodes)
            {
                if (!node.Visited)
                    Sort(node);
            }
        }

        void Sort(TNode node)
        {
            node.Visited = true;
            foreach (Edge<T, TNode> edge in _list.GetEdges(node))
            {
                if (!edge.Target.Visited)
                    Sort(edge.Target);
            }

            _results.Add(node);
        }
    }
}
