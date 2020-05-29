namespace MassTransit
{
    using System;
    using GreenPipes.Caching;


    public class CacheConfigurator :
        ICacheConfigurator
    {
        public CacheConfigurator()
        {
            Settings = new CacheSettings();
        }

        public int Capacity
        {
            set => Settings.Capacity = value;
        }

        public TimeSpan MinAge
        {
            set => Settings.MinAge = value;
        }

        public TimeSpan MaxAge
        {
            set => Settings.MaxAge = value;
        }

        public CacheSettings Settings { get; }
    }
}
