namespace MassTransit.Internals.Caching
{
    using System;
    using System.Diagnostics;


    public class TimeToLiveCachePolicy<TValue> :
        ICachePolicy<TValue, ITimeToLiveCacheValue<TValue>>
        where TValue : class
    {
        readonly long _timeToLive;

        public TimeToLiveCachePolicy(TimeSpan timeToLive)
        {
            _timeToLive = timeToLive.Ticks;
        }

        public ITimeToLiveCacheValue<TValue> CreateValue(Action remove)
        {
            return new TimeToLiveCacheValue<TValue>(remove, Stopwatch.GetTimestamp());
        }

        public bool IsValid(ITimeToLiveCacheValue<TValue> value)
        {
            return Stopwatch.GetTimestamp() - value.Timestamp <= _timeToLive;
        }

        public int CheckValue(ITimeToLiveCacheValue<TValue> value)
        {
            var usage = value.Usage;

            return IsValid(value)
                ? usage
                : int.MinValue;
        }
    }
}
