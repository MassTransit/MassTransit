namespace MassTransit
{
    using System;
    using GreenPipes.Caching;


    public interface ICacheConfigurator
    {
        int Capacity { set; }
        TimeSpan MinAge { set; }
        TimeSpan MaxAge { set; }
        CacheSettings Settings { get; }
    }
}
