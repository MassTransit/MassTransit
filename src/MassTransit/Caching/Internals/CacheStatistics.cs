namespace MassTransit.Caching.Internals
{
    using System;
    using System.Threading;


    public class CacheStatistics
    {
        int _count;
        int _createFaults;
        long _hits;
        long _misses;
        int _totalCount;

        public CacheStatistics(int capacity, int bucketCount, int bucketSize, TimeSpan minAge, TimeSpan maxAge, TimeSpan validityCheckInterval)
        {
            Capacity = capacity;
            BucketCount = bucketCount;
            BucketSize = bucketSize;
            MinAge = minAge;
            MaxAge = maxAge;
            ValidityCheckInterval = validityCheckInterval;
        }

        public TimeSpan ValidityCheckInterval { get; }

        /// <summary>
        /// How long a value can live in the cache until being swept during the next cleanup
        /// </summary>
        public TimeSpan MaxAge { get; }

        /// <summary>
        /// The shortest time a value can live in the cache, even if it means blowing up the cache size
        /// </summary>
        public TimeSpan MinAge { get; }

        /// <summary>
        /// How many values each bucket should hold
        /// </summary>
        public int BucketSize { get; }

        /// <summary>
        /// How much buckets are maintained
        /// </summary>
        public int BucketCount { get; }

        /// <summary>
        /// The lowest bucket index with nodes in it
        /// </summary>
        public int OldestBucketIndex { get; private set; }

        /// <summary>
        /// The current bucket for nodes
        /// </summary>
        public int CurrentBucketIndex { get; private set; }

        /// <summary>
        /// The value limit for the cache
        /// </summary>
        /// <remarks>
        /// The actual number of values can exceed the limit if items are being added quickly and take a while to reach the minimum age
        /// </remarks>
        public int Capacity { get; }

        /// <summary>
        /// Current value count
        /// </summary>
        public int Count => _count;

        /// <summary>
        /// Total number of values added to the cache since it was created
        /// </summary>
        public int TotalCount => _totalCount;

        /// <summary>
        /// Gets the number of times an item was requested from the cache which did not exist yet, since the cache
        /// was created.
        /// </summary>
        public long Misses => _misses;

        /// <summary>
        /// Gets the number of times an existing item was requested from the cache since the cache
        /// was created.
        /// </summary>
        public long Hits => _hits;

        /// <summary>
        /// The number of node creates which faulted
        /// </summary>
        public int CreateFaults => _createFaults;

        /// <summary>
        /// Resets the statistics.
        /// </summary>
        public void Reset()
        {
            _totalCount = 0;
            _misses = 0;
            _hits = 0;
            _count = 0;
        }

        internal void ValueAdded()
        {
            Interlocked.Increment(ref _totalCount);
            Interlocked.Increment(ref _count);
        }

        internal void ValueRemoved()
        {
            Interlocked.Decrement(ref _count);
        }

        internal void Miss()
        {
            Interlocked.Increment(ref _misses);
        }

        internal void Hit()
        {
            Interlocked.Increment(ref _hits);
        }

        public override string ToString()
        {
            return $"Count: {Count}, Total: {TotalCount}, Hits: {Hits}, Misses: {Misses}, Faults: {CreateFaults}, Capacity: {Capacity}";
        }

        internal void SetBucketIndices(int oldestBucketIndex, int currentBucketIndex)
        {
            CurrentBucketIndex = currentBucketIndex % BucketCount;
            OldestBucketIndex = oldestBucketIndex % BucketCount;
        }

        public void CreateFaulted()
        {
            Interlocked.Increment(ref _createFaults);
        }
    }
}
