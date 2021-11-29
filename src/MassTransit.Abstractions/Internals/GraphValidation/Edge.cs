namespace MassTransit.Internals.GraphValidation
{
    using System;


    public struct Edge<T, TNode> :
        IComparable<Edge<T, TNode>>
        where TNode : Node<T>
    {
        public readonly TNode Source;
        public readonly TNode Target;
        public readonly int Weight;

        public Edge(TNode source, TNode target, int weight)
        {
            Source = source;
            Target = target;
            Weight = weight;
        }

        public int CompareTo(Edge<T, TNode> other)
        {
            return Weight - other.Weight;
        }
    }
}
