namespace MassTransit.Configuration
{
    using System;
    using DependencyInjection.Registration;
    using Internals;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;


    public static class DependencyInjectionSagaRegistrationExtensions
    {
        public static ISagaRegistration RegisterSaga<T>(this IServiceCollection collection)
            where T : class, ISaga
        {
            return RegisterSaga<T>(collection, new DependencyInjectionContainerRegistrar(collection));
        }

        public static ISagaRegistration RegisterSaga<T>(this IServiceCollection collection, IContainerRegistrar registrar)
            where T : class, ISaga
        {
            if (typeof(T).HasInterface<SagaStateMachineInstance>())
                throw new ArgumentException($"State machine sagas must be registered using RegisterSagaStateMachine: {TypeCache<T>.ShortName}");

            return new SagaRegistrar<T>().Register(collection, registrar);
        }

        public static ISagaRegistration RegisterSaga<T, TDefinition>(this IServiceCollection collection)
            where T : class, ISaga
            where TDefinition : class, ISagaDefinition<T>
        {
            return RegisterSaga<T, TDefinition>(collection, new DependencyInjectionContainerRegistrar(collection));
        }

        public static ISagaRegistration RegisterSaga<T, TDefinition>(this IServiceCollection collection, IContainerRegistrar registrar)
            where T : class, ISaga
            where TDefinition : class, ISagaDefinition<T>
        {
            if (typeof(T).HasInterface<SagaStateMachineInstance>())
                throw new ArgumentException($"State machine sagas must be registered using RegisterSagaStateMachine: {TypeCache<T>.ShortName}");

            return new SagaDefinitionRegistrar<T, TDefinition>().Register(collection, registrar);
        }

        public static ISagaRegistration RegisterSaga<T>(this IServiceCollection collection, Type sagaDefinitionType)
            where T : class, ISaga
        {
            return RegisterSaga<T>(collection, new DependencyInjectionContainerRegistrar(collection), sagaDefinitionType);
        }

        public static ISagaRegistration RegisterSaga<T>(this IServiceCollection collection, IContainerRegistrar registrar, Type sagaDefinitionType)
            where T : class, ISaga
        {
            if (sagaDefinitionType == null)
                return RegisterSaga<T>(collection, registrar);

            if (typeof(T).HasInterface<SagaStateMachineInstance>())
                throw new ArgumentException($"State machine sagas must be registered using RegisterSagaStateMachine: {TypeCache<T>.ShortName}");

            if (!sagaDefinitionType.ClosesType(typeof(ISagaDefinition<>), out Type[] types) || types[0] != typeof(T))
            {
                throw new ArgumentException($"{TypeCache.GetShortName(sagaDefinitionType)} is not a saga definition of {TypeCache<T>.ShortName}",
                    nameof(sagaDefinitionType));
            }

            var register = (ISagaRegistrar)Activator.CreateInstance(typeof(SagaDefinitionRegistrar<,>).MakeGenericType(typeof(T), sagaDefinitionType));

            return register.Register(collection, registrar);
        }


        interface ISagaRegistrar
        {
            ISagaRegistration Register(IServiceCollection collection, IContainerRegistrar registrar);
        }


        class SagaRegistrar<TSaga> :
            ISagaRegistrar
            where TSaga : class, ISaga
        {
            public virtual ISagaRegistration Register(IServiceCollection collection, IContainerRegistrar registrar)
            {
                return registrar.GetOrAdd<ISagaRegistration>(typeof(TSaga), _ => new SagaRegistration<TSaga>());
            }
        }


        class SagaDefinitionRegistrar<TSaga, TDefinition> :
            SagaRegistrar<TSaga>
            where TDefinition : class, ISagaDefinition<TSaga>
            where TSaga : class, ISaga
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
