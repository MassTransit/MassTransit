namespace MassTransit.Conductor.Client
{
    using System;
    using GreenPipes.Caching;


    /// <summary>
    /// Used to configure the defaults for a service instance to cache service clients
    /// </summary>
    public static class ServiceClientCacheDefaults
    {
        static ServiceClientCacheDefaults()
        {
            Capacity = 1000;
            MinAge = TimeSpan.FromSeconds(10);
            MaxAge = TimeSpan.FromMinutes(30);
        }

        public static int Capacity { get; set; }
        public static TimeSpan MinAge { get; set; }
        public static TimeSpan MaxAge { get; set; }

        public static CacheSettings Settings => new CacheSettings(Capacity, MinAge, MaxAge);
    }
}
