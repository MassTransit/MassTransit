namespace MassTransit.SimpleInjectorIntegration
{
    using System;
    using System.Collections.Concurrent;

    using SimpleInjector;

    public static class ConsumerConfiguratorCache
    {
        static CachedConfigurator GetOrAdd(Type type)
        {
            return Cached.Instance.GetOrAdd(type, _ =>
                                                  (CachedConfigurator)Activator.CreateInstance(typeof(CachedConfigurator<>).MakeGenericType(type)));
        }

        public static void Configure(Type consumerType, IReceiveEndpointConfigurator configurator, Container container)
        {
            GetOrAdd(consumerType).Configure(configurator, container);
        }
        
        interface CachedConfigurator
        {
            void Configure(IReceiveEndpointConfigurator configurator, Container container);
        }
        
        class CachedConfigurator<T> :
            CachedConfigurator
            where T : class, IConsumer
        {
            public void Configure(IReceiveEndpointConfigurator configurator, Container container)
            {
                configurator.Consumer(new SimpleInjectorConsumerFactory<T>(container));
            }
        }
        
        static class Cached
        {
            internal static readonly ConcurrentDictionary<Type, CachedConfigurator> Instance =
                new ConcurrentDictionary<Type, CachedConfigurator>();
        }
    }
}