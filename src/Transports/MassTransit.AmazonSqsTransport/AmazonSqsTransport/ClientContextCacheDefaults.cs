namespace MassTransit.AmazonSqsTransport
{
    using System;
    using Internals.Caching;


    public static class ClientContextCacheDefaults
    {
        static ClientContextCacheDefaults()
        {
            Capacity = 1000;
            MinAge = TimeSpan.FromHours(1);
            MaxAge = TimeSpan.FromDays(427);
        }

        public static int Capacity { get; set; }
        public static TimeSpan MinAge { get; set; }
        public static TimeSpan MaxAge { get; set; }

        public static ICache<TKey, TValue, ITimeToLiveCacheValue<TValue>> CreateCache<TKey, TValue>()
            where TValue : class
        {
            var options = new CacheOptions { Capacity = Capacity };
            var policy = new TimeToLiveCachePolicy<TValue>(MaxAge);

            return new MassTransitCache<TKey, TValue, ITimeToLiveCacheValue<TValue>>(policy, options);
        }
    }
}
