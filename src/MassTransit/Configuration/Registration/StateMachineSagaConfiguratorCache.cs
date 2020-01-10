namespace MassTransit.Registration
{
    using System;
    using System.Collections.Concurrent;
    using Automatonymous;
    using Saga;


    public static class StateMachineSagaConfiguratorCache
    {
        static CachedConfigurator GetOrAdd(Type type)
        {
            return Cached.Instance.GetOrAdd(type, _ => (CachedConfigurator)Activator.CreateInstance(typeof(CachedConfigurator<>).MakeGenericType(type)));
        }

        public static void Configure(Type sagaType, IReceiveEndpointConfigurator configurator, ISagaStateMachineFactory sagaStateMachineFactory,
            ISagaRepositoryFactory repositoryFactory)
        {
            GetOrAdd(sagaType).Configure(configurator, sagaStateMachineFactory, repositoryFactory);
        }


        static class Cached
        {
            internal static readonly ConcurrentDictionary<Type, CachedConfigurator> Instance = new ConcurrentDictionary<Type, CachedConfigurator>();
        }


        interface CachedConfigurator
        {
            void Configure(IReceiveEndpointConfigurator configurator, ISagaStateMachineFactory sagaStateMachineFactory,
                ISagaRepositoryFactory repositoryFactory);
        }


        class CachedConfigurator<T> :
            CachedConfigurator
            where T : class, SagaStateMachineInstance
        {
            public void Configure(IReceiveEndpointConfigurator configurator, ISagaStateMachineFactory sagaStateMachineFactory,
                ISagaRepositoryFactory repositoryFactory)
            {
                ISagaRepository<T> repository = repositoryFactory.CreateSagaRepository<T>();

                var stateMachine = sagaStateMachineFactory.CreateStateMachine<T>();

                configurator.StateMachineSaga(stateMachine, repository);
            }
        }
    }
}
