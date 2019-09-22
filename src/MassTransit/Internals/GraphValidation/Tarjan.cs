namespace MassTransit.Internals.GraphValidation
{
    using System;
    using System.Collections.Generic;


    class Tarjan<T, TNode>
        where TNode : Node<T>, ITarjanNodeProperties
    {
        readonly AdjacencyList<T, TNode> _list;
        readonly IList<IList<TNode>> _result;
        readonly Stack<TNode> _stack;
        int _index;

        public Tarjan(AdjacencyList<T, TNode> list)
        {
            _list = list;
            _index = 0;
            _result = new List<IList<TNode>>();
            _stack = new Stack<TNode>();

            foreach (TNode node in _list.SourceNodes)
            {
                if (node.Index != -1)
                    continue;

                Compute(node);
            }
        }

        public IList<IList<TNode>> Result => _result;

        void Compute(TNode v)
        {
            v.Index = _index;
            v.LowLink = _index;
            _index++;

            _stack.Push(v);

            foreach (var edge in _list.GetEdges(v))
            {
                TNode n = edge.Target;
                if (n.Index == -1)
                {
                    Compute(n);
                    v.LowLink = Math.Min(v.LowLink, n.LowLink);
                }
                else if (_stack.Contains(n))
                    v.LowLink = Math.Min(v.LowLink, n.Index);
            }

            if (v.LowLink == v.Index)
            {
                TNode n;
                IList<TNode> component = new List<TNode>();
                do
                {
                    n = _stack.Pop();
                    component.Add(n);
                }
                while (!v.Equals(n));

                if (component.Count != 1 || !v.Equals(component[0]))
                    _result.Add(component);
            }
        }
    }
}