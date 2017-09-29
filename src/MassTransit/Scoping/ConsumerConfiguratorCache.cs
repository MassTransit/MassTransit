namespace MassTransit.Scoping
{
    using System;
    using System.Collections.Concurrent;


    public static class ConsumerConfiguratorCache
    {
        static CachedConfigurator GetOrAdd(Type type)
        {
            return Cached.Instance.GetOrAdd(type, _ =>
                (CachedConfigurator)Activator.CreateInstance(typeof(CachedConfigurator<>).MakeGenericType(type)));
        }

        public static void Configure(Type consumerType, IReceiveEndpointConfigurator configurator, IConsumerScopeProvider scopeProvider)
        {
            GetOrAdd(consumerType).Configure(configurator, scopeProvider);
        }


        interface CachedConfigurator
        {
            void Configure(IReceiveEndpointConfigurator configurator, IConsumerScopeProvider scopeProvider);
        }


        class CachedConfigurator<T> :
            CachedConfigurator
            where T : class, IConsumer
        {
            public void Configure(IReceiveEndpointConfigurator configurator, IConsumerScopeProvider scopeProvider)
            {
                var consumerFactory = new ScopeConsumerFactory<T>(scopeProvider);
                
                configurator.Consumer(consumerFactory);
            }
        }


        static class Cached
        {
            internal static readonly ConcurrentDictionary<Type, CachedConfigurator> Instance =
                new ConcurrentDictionary<Type, CachedConfigurator>();
        }
    }
}