namespace MassTransit.Internals.Caching
{
    using System;


    public class TimeToLiveCacheValue<TValue> :
        CacheValue<TValue>,
        ITimeToLiveCacheValue<TValue>
        where TValue : class
    {
        public TimeToLiveCacheValue(Action remove, long timestamp)
            : base(remove)
        {
            Timestamp = timestamp;
        }

        public long Timestamp { get; set; }
    }
}
