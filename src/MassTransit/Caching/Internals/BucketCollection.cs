namespace MassTransit.Caching.Internals
{
    using System;
    using System.Linq;


    /// <summary>
    /// An ordered collection of buckets, used by the node tracker to keep track of nodes
    /// </summary>
    /// <typeparam name="TValue">The value type</typeparam>
    public class BucketCollection<TValue>
        where TValue : class
    {
        readonly Bucket<TValue>[] _buckets;

        public BucketCollection(INodeTracker<TValue> nodeTracker, int capacity)
        {
            _buckets = Enumerable.Range(0, capacity)
                .Select(index => new Bucket<TValue>(nodeTracker))
                .ToArray();
        }

        public Bucket<TValue> this[int index]
        {
            get
            {
                if (index == int.MaxValue)
                    throw new OverflowException("The bucket limit has been reached, the cache is dead.");

                if (index < 0)
                    throw new ArgumentOutOfRangeException(nameof(index), "The bucket index must be >= 0.");

                return _buckets[index % _buckets.Length];
            }
        }

        public int Count => _buckets.Length;

        /// <summary>
        /// Empties every bucket in the collection, evicting all the nodes
        /// </summary>
        public void Empty()
        {
            foreach (Bucket<TValue> bucket in _buckets)
            {
                IBucketNode<TValue> node = bucket.Head;
                bucket.Clear();

                while (node != null)
                {
                    IBucketNode<TValue> next = node.Pop();
                    node.Evict();
                    node = next;
                }
            }
        }
    }
}
