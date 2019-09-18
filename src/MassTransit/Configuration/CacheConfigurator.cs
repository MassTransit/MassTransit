namespace MassTransit
{
    using System;
    using GreenPipes.Caching;


    public class CacheConfigurator :
        ICacheConfigurator
    {
        readonly CacheSettings _settings;

        public CacheConfigurator()
        {
            _settings = new CacheSettings();
        }

        public int Capacity
        {
            set => _settings.Capacity = value;
        }

        public TimeSpan MinAge
        {
            set => _settings.MinAge = value;
        }

        public TimeSpan MaxAge
        {
            set => _settings.MaxAge = value;
        }

        public CacheSettings Settings => _settings;
    }
}
