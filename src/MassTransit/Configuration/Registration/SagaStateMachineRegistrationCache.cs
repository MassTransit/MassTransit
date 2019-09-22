namespace MassTransit.Registration
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using Automatonymous;
    using Internals.Extensions;
    using Metadata;


    public static class SagaStateMachineRegistrationCache
    {
        static CachedRegistration GetOrAdd(Type type)
        {
            if (!type.HasInterface(typeof(SagaStateMachine<>)))
                throw new ArgumentException($"The type is not a state machine saga: {TypeMetadataCache.GetShortName(type)}", nameof(type));

            var instanceType = type.GetClosingArguments(typeof(SagaStateMachine<>)).Single();

            return Cached.Instance.GetOrAdd(instanceType,
                _ => (CachedRegistration)Activator.CreateInstance(typeof(CachedRegistration<,>).MakeGenericType(type, instanceType)));
        }

        public static void Register(Type stateMachineType, IContainerRegistrar registrar)
        {
            GetOrAdd(stateMachineType).Register(registrar);
        }

        public static void AddSagaStateMachine(IRegistrationConfigurator configurator, Type stateMachineType, Type sagaDefinitionType = null)
        {
            GetOrAdd(stateMachineType).AddSaga(configurator, sagaDefinitionType);
        }


        static class Cached
        {
            internal static readonly ConcurrentDictionary<Type, CachedRegistration> Instance = new ConcurrentDictionary<Type, CachedRegistration>();
        }


        interface CachedRegistration
        {
            void Register(IContainerRegistrar registrar);
            void AddSaga(IRegistrationConfigurator registry, Type sagaDefinitionType);
        }


        class CachedRegistration<TStateMachine, TInstance> :
            CachedRegistration
            where TStateMachine : class, SagaStateMachine<TInstance>
            where TInstance : class, SagaStateMachineInstance
        {
            public void Register(IContainerRegistrar registrar)
            {
                registrar.RegisterStateMachineSaga<TStateMachine, TInstance>();

                SagaRegistrationCache.DoNotRegister(typeof(TInstance));
            }

            public void AddSaga(IRegistrationConfigurator registry, Type sagaDefinitionType)
            {
                registry.AddSagaStateMachine<TStateMachine, TInstance>(sagaDefinitionType);
            }
        }
    }
}
