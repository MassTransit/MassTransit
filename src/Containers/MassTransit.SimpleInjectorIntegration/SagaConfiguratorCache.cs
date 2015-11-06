namespace MassTransit.SimpleInjectorIntegration
{
    using System;
    using System.Collections.Concurrent;

    using MassTransit.Saga;

    using SimpleInjector;


    public static class SagaConfiguratorCache
    {
        static CachedConfigurator GetOrAdd(Type type)
        {
            return Cached.Instance.GetOrAdd(type, _ =>
                                                  (CachedConfigurator)Activator.CreateInstance(typeof(CachedConfigurator<>).MakeGenericType(type)));
        }

        public static void Configure(Type sagaType, IReceiveEndpointConfigurator configurator, Container container)
        {
            GetOrAdd(sagaType).Configure(configurator, container);
        }


        static class Cached
        {
            internal static readonly ConcurrentDictionary<Type, CachedConfigurator> Instance =
                new ConcurrentDictionary<Type, CachedConfigurator>();
        }


        interface CachedConfigurator
        {
            void Configure(IReceiveEndpointConfigurator configurator, Container container);
        }


        class CachedConfigurator<T> :
            CachedConfigurator
            where T : class, ISaga
        {
            public void Configure(IReceiveEndpointConfigurator configurator, Container container)
            {
                var sagaRepository = container.GetInstance<ISagaRepository<T>>();

                var simpleInjectorSagaRepository = new SimpleInjectorSagaRepository<T>(sagaRepository, container);

                configurator.Saga(simpleInjectorSagaRepository);
            }
        }
    }
}