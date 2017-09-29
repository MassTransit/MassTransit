namespace MassTransit.Scoping
{
    using System;
    using System.Collections.Concurrent;
    using Saga;


    public static class SagaConfiguratorCache
    {
        static CachedConfigurator GetOrAdd(Type type)
        {
            return Cached.Instance.GetOrAdd(type, _ =>
                (CachedConfigurator)Activator.CreateInstance(typeof(CachedConfigurator<>).MakeGenericType(type)));
        }

        public static void Configure(Type sagaType, IReceiveEndpointConfigurator configurator, ISagaRepositoryFactory factory)
        {
            GetOrAdd(sagaType).Configure(configurator, factory);
        }


        static class Cached
        {
            internal static readonly ConcurrentDictionary<Type, CachedConfigurator> Instance =
                new ConcurrentDictionary<Type, CachedConfigurator>();
        }


        interface CachedConfigurator
        {
            void Configure(IReceiveEndpointConfigurator configurator, ISagaRepositoryFactory factory);
        }


        class CachedConfigurator<T> :
            CachedConfigurator
            where T : class, ISaga
        {
            public void Configure(IReceiveEndpointConfigurator configurator, ISagaRepositoryFactory factory)
            {
                var repository = factory.CreateSagaRepository<T>();

                configurator.Saga(repository);
            }
        }
    }
}