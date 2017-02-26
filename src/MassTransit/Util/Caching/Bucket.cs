namespace MassTransit.Util.Caching
{
    using System;
    using System.Threading;


    public class Bucket<TValue>
        where TValue : class
    {
        readonly INodeTracker<TValue> _tracker;
        DateTime _startTime;
        DateTime? _stopTime;
        IBucketNode<TValue> _first;
        int _count;

        public Bucket(INodeTracker<TValue> tracker)
        {
            _tracker = tracker;
        }

        public bool HasExpired(TimeSpan maxAge, DateTime now)
        {
            return _startTime < now.Subtract(maxAge);
        }

        public bool IsOldEnough(TimeSpan minAge, DateTime now)
        {
            return now - _stopTime > minAge;
        }

        public void Empty()
        {
            _first = null;
            _count = 0;
        }

        public IBucketNode<TValue> First => _first;

        public void Stop(DateTime now)
        {
            _stopTime = now;
        }

        public void Start(DateTime now)
        {
            _startTime = now;
            _stopTime = default(DateTime?);

            _first = null;
            _count = 0;
        }

        public int Count => _count;

        public IBucketNode<TValue> Push(IBucketNode<TValue> node)
        {
            var next = _first;

            _first = node;

            node.SetBucket(this, next);

            Interlocked.Increment(ref _count);

            return next;
        }

        public void Touch(IBucketNode<TValue> node)
        {
            // a stopped bucket is no longer the current bucket, so give the node back to the manager
            if (_stopTime.HasValue)
            {
                _tracker.Rebucket(node);

                Interlocked.Decrement(ref _count);
            }
        }
    }
}