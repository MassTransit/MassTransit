namespace MassTransit.Caching.Internals
{
    using System;
    using System.Diagnostics;
    using System.Threading;


    public class Bucket<TValue>
        where TValue : class
    {
        readonly INodeTracker<TValue> _tracker;
        int _count;
        IBucketNode<TValue> _head;
        DateTime _startTime;
        DateTime? _stopTime;

        public Bucket(INodeTracker<TValue> tracker)
        {
            _tracker = tracker;
        }

        public IBucketNode<TValue> Head => _head;

        public int Count => _count;

        public bool HasExpired(DateTime expirationTime)
        {
            return _startTime < expirationTime;
        }

        public bool IsOldEnough(DateTime agedTime)
        {
            return _stopTime < agedTime;
        }

        /// <summary>
        /// Clear the bucket, no node cleanup is performed
        /// </summary>
        public void Clear()
        {
            _head = null;
            _count = 0;
        }

        public void Stop(DateTime now)
        {
            _stopTime = now;
        }

        public void Start(DateTime now)
        {
            _startTime = now;
            _stopTime = default;

            _head = null;
            _count = 0;
        }

        /// <summary>
        /// Push a node to the front of the bucket, and set the node's bucket to this bucket
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public void Push(IBucketNode<TValue> node)
        {
            Debug.Assert(_stopTime.HasValue == false, "Bucket is stopped");

            IBucketNode<TValue> next;
            do
            {
                next = _head;

                node.SetBucket(this, next);
            }
            while (next != Interlocked.CompareExchange(ref _head, node, next));

            Interlocked.Increment(ref _count);
        }

        /// <summary>
        /// When a node is used, check and rebucket if necessary to keep it in the cache
        /// </summary>
        /// <param name="node"></param>
        public void Used(IBucketNode<TValue> node)
        {
            // a stopped bucket is no longer the current bucket, so give the node back to the manager
            if (!_stopTime.HasValue)
                return;

            _tracker.Rebucket(node);

            Interlocked.Decrement(ref _count);
        }
    }
}
