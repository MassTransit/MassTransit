namespace MassTransit.Monitoring.Performance
{
    using System;
    using System.Collections.Concurrent;


    public class ConsumerPerformanceCounterCache
    {
        readonly ConcurrentDictionary<string, Lazy<ConsumerPerformanceCounter>> _types;

        ConsumerPerformanceCounterCache()
        {
            _types = new ConcurrentDictionary<string, Lazy<ConsumerPerformanceCounter>>();
        }

        public static IConsumerPerformanceCounter GetCounter(ICounterFactory factory, string consumerType)
        {
            return Cached.Counter.Value._types
                .GetOrAdd(consumerType, x => new Lazy<ConsumerPerformanceCounter>(() => new ConsumerPerformanceCounter(factory, x)))
                .Value;
        }


        static class Cached
        {
            public static readonly Lazy<ConsumerPerformanceCounterCache> Counter =
                new Lazy<ConsumerPerformanceCounterCache>(() => new ConsumerPerformanceCounterCache());
        }
    }
}
