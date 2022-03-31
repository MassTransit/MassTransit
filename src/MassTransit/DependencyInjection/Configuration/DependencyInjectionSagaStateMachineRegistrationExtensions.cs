namespace MassTransit.Configuration
{
    using System;
    using DependencyInjection.Registration;
    using Internals;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;


    public static class DependencyInjectionSagaStateMachineRegistrationExtensions
    {
        public static ISagaRegistration RegisterSagaStateMachine<T, TSaga>(this IServiceCollection collection)
            where T : class, SagaStateMachine<TSaga>
            where TSaga : class, SagaStateMachineInstance
        {
            return RegisterSagaStateMachine<T, TSaga>(collection, new DependencyInjectionContainerRegistrar(collection));
        }

        public static ISagaRegistration RegisterSagaStateMachine<T, TSaga>(this IServiceCollection collection, IContainerRegistrar registrar)
            where T : class, SagaStateMachine<TSaga>
            where TSaga : class, SagaStateMachineInstance
        {
            return new SagaRegistrar<T, TSaga>().Register(collection, registrar);
        }

        public static ISagaRegistration RegisterSaga<T, TSaga, TDefinition>(this IServiceCollection collection)
            where T : class, SagaStateMachine<TSaga>
            where TSaga : class, SagaStateMachineInstance
            where TDefinition : class, ISagaDefinition<TSaga>
        {
            return RegisterSaga<T, TSaga, TDefinition>(collection, new DependencyInjectionContainerRegistrar(collection));
        }

        public static ISagaRegistration RegisterSaga<T, TSaga, TDefinition>(this IServiceCollection collection, IContainerRegistrar registrar)
            where T : class, SagaStateMachine<TSaga>
            where TSaga : class, SagaStateMachineInstance
            where TDefinition : class, ISagaDefinition<TSaga>
        {
            return new SagaDefinitionRegistrar<T, TSaga, TDefinition>().Register(collection, registrar);
        }

        public static ISagaRegistration RegisterSagaStateMachine<T, TSaga>(this IServiceCollection collection, Type sagaDefinitionType)
            where T : class, SagaStateMachine<TSaga>
            where TSaga : class, SagaStateMachineInstance
        {
            return RegisterSagaStateMachine<T, TSaga>(collection, new DependencyInjectionContainerRegistrar(collection), sagaDefinitionType);
        }

        public static ISagaRegistration RegisterSagaStateMachine<T, TSaga>(this IServiceCollection collection, IContainerRegistrar registrar,
            Type sagaDefinitionType)
            where T : class, SagaStateMachine<TSaga>
            where TSaga : class, SagaStateMachineInstance
        {
            if (sagaDefinitionType == null)
                return RegisterSagaStateMachine<T, TSaga>(collection, registrar);

            if (!sagaDefinitionType.ClosesType(typeof(ISagaDefinition<>), out Type[] types) || types[0] != typeof(TSaga))
            {
                throw new ArgumentException($"{TypeCache.GetShortName(sagaDefinitionType)} is not a saga definition of {TypeCache<TSaga>.ShortName}",
                    nameof(sagaDefinitionType));
            }

            var register = (ISagaRegistrar)Activator.CreateInstance(
                typeof(SagaDefinitionRegistrar<,,>).MakeGenericType(typeof(T), typeof(TSaga), sagaDefinitionType));

            return register.Register(collection, registrar);
        }


        interface ISagaRegistrar
        {
            ISagaRegistration Register(IServiceCollection collection, IContainerRegistrar registrar);
        }


        class SagaRegistrar<TStateMachine, TSaga> :
            ISagaRegistrar
            where TStateMachine : class, SagaStateMachine<TSaga>
            where TSaga : class, SagaStateMachineInstance
        {
            public virtual ISagaRegistration Register(IServiceCollection collection, IContainerRegistrar registrar)
            {
                collection.AddSingleton<TStateMachine>();
                collection.AddSingleton<SagaStateMachine<TSaga>>(provider => provider.GetRequiredService<TStateMachine>());

                return registrar.GetOrAdd<ISagaRegistration>(typeof(TSaga), _ => new SagaStateMachineRegistration<TSaga>());
            }
        }


        class SagaDefinitionRegistrar<TStateMachine, TSaga, TDefinition> :
            SagaRegistrar<TStateMachine, TSaga>
            where TStateMachine : class, SagaStateMachine<TSaga>
            where TSaga : class, SagaStateMachineInstance
            where TDefinition : class, ISagaDefinition<TSaga>
        {
            public override ISagaRegistration Register(IServiceCollection collection, IContainerRegistrar registrar)
            {
                var registration = base.Register(collection, registrar);

                collection.TryAddSingleton<TDefinition>();
                collection.TryAddSingleton<ISagaDefinition<TSaga>>(provider => provider.GetRequiredService<TDefinition>());

                return registration;
            }
        }
    }
}
