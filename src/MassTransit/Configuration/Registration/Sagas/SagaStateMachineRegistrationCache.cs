namespace MassTransit.Registration
{
    using System;
    using System.Linq;
    using Automatonymous;
    using Internals.Extensions;
    using Metadata;


    public static class SagaStateMachineRegistrationCache
    {
        public static void Register(Type stateMachineType, IContainerRegistrar registrar)
        {
            Cached.Instance.GetOrAdd(stateMachineType).Register(registrar);
        }

        public static void AddSagaStateMachine(IRegistrationConfigurator configurator, Type stateMachineType, ISagaRepositoryRegistrationProvider provider,
            Type sagaDefinitionType = null)
        {
            Cached.Instance.GetOrAdd(stateMachineType).AddSaga(configurator, provider, sagaDefinitionType);
        }

        static CachedRegistration Factory(Type type)
        {
            if (!type.HasInterface(typeof(SagaStateMachine<>)))
                throw new ArgumentException($"The type is not a state machine saga: {TypeMetadataCache.GetShortName(type)}", nameof(type));

            var instanceType = type.GetClosingArguments(typeof(SagaStateMachine<>)).Single();

            return (CachedRegistration)Activator.CreateInstance(typeof(CachedRegistration<,>).MakeGenericType(type, instanceType));
        }


        static class Cached
        {
            internal static readonly RegistrationCache<CachedRegistration> Instance = new RegistrationCache<CachedRegistration>(Factory);
        }


        interface CachedRegistration
        {
            void Register(IContainerRegistrar registrar);
            void AddSaga(IRegistrationConfigurator registry, ISagaRepositoryRegistrationProvider provider, Type sagaDefinitionType);
        }


        class CachedRegistration<TStateMachine, TInstance> :
            CachedRegistration
            where TStateMachine : class, SagaStateMachine<TInstance>
            where TInstance : class, SagaStateMachineInstance
        {
            public void Register(IContainerRegistrar registrar)
            {
                registrar.RegisterSagaStateMachine<TStateMachine, TInstance>();

                SagaRegistrationCache.DoNotRegister(typeof(TInstance));
            }

            public void AddSaga(IRegistrationConfigurator registry, ISagaRepositoryRegistrationProvider provider, Type sagaDefinitionType)
            {
                var configurator = registry.AddSagaStateMachine<TStateMachine, TInstance>(sagaDefinitionType);

                provider?.Configure(configurator);
            }
        }
    }
}
