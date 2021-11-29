namespace MassTransit.AmazonSqsTransport
{
    using System;
    using Caching;


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

        public static CacheSettings GetCacheSettings()
        {
            return new CacheSettings(Capacity, MinAge, MaxAge);
        }
    }
}
