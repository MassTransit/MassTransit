namespace MassTransit.Configuration
{
    using System;
    using DependencyInjection.Registration;
    using Internals;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;


    public static class DependencyInjectionFutureRegistrationExtensions
    {
        public static IFutureRegistration RegisterFuture<T>(this IServiceCollection collection)
            where T : class, SagaStateMachine<FutureState>
        {
            return RegisterFuture<T, DefaultFutureDefinition<T>>(collection, new DependencyInjectionContainerRegistrar(collection));
        }

        public static IFutureRegistration RegisterFuture<T>(this IServiceCollection collection, IContainerRegistrar registrar)
            where T : class, SagaStateMachine<FutureState>
        {
            return RegisterFuture<T, DefaultFutureDefinition<T>>(collection, registrar);
        }

        public static IFutureRegistration RegisterFuture<T, TDefinition>(this IServiceCollection collection)
            where T : class, SagaStateMachine<FutureState>
            where TDefinition : class, IFutureDefinition<T>
        {
            return RegisterFuture<T, TDefinition>(collection, new DependencyInjectionContainerRegistrar(collection));
        }

        public static IFutureRegistration RegisterFuture<T, TDefinition>(this IServiceCollection collection, IContainerRegistrar registrar)
            where T : class, SagaStateMachine<FutureState>
            where TDefinition : class, IFutureDefinition<T>
        {
            return new FutureDefinitionRegistrar<T, TDefinition>().Register(collection, registrar);
        }

        public static IFutureRegistration RegisterFuture<T>(this IServiceCollection collection, Type futureDefinitionType)
            where T : class, SagaStateMachine<FutureState>
        {
            return RegisterFuture<T>(collection, new DependencyInjectionContainerRegistrar(collection), futureDefinitionType);
        }

        public static IFutureRegistration RegisterFuture<T>(this IServiceCollection collection, IContainerRegistrar registrar, Type futureDefinitionType)
            where T : class, SagaStateMachine<FutureState>
        {
            if (futureDefinitionType == null)
                return RegisterFuture<T, DefaultFutureDefinition<T>>(collection, registrar);

            if (!futureDefinitionType.ClosesType(typeof(IFutureDefinition<>), out Type[] types) || types[0] != typeof(T))
            {
                throw new ArgumentException($"{TypeCache.GetShortName(futureDefinitionType)} is not a future definition of {TypeCache<T>.ShortName}",
                    nameof(futureDefinitionType));
            }

            var register = (IFutureRegistrar)Activator.CreateInstance(typeof(FutureDefinitionRegistrar<,>).MakeGenericType(typeof(T), futureDefinitionType));

            return register.Register(collection, registrar);
        }

        public static IFutureRegistration RegisterFuture(this IServiceCollection collection, IContainerRegistrar registrar, Type futureType,
            Type futureDefinitionType = null)
        {
            if (!futureType.HasInterface<SagaStateMachine<FutureState>>())
                throw new ArgumentException($"The registered type must be a future: {TypeCache.GetShortName(futureType)}");

            futureDefinitionType ??= typeof(DefaultFutureDefinition<>).MakeGenericType(futureType);

            if (!futureDefinitionType.ClosesType(typeof(ISagaDefinition<>), out Type[] types) || types[0] != futureType)
            {
                throw new ArgumentException(
                    $"{TypeCache.GetShortName(futureDefinitionType)} is not a future definition of {TypeCache.GetShortName(futureType)}",
                    nameof(futureDefinitionType));
            }

            var sagaRegistrar =
                (IFutureRegistrar)Activator.CreateInstance(typeof(FutureDefinitionRegistrar<,>).MakeGenericType(futureType, futureDefinitionType));

            return sagaRegistrar.Register(collection, registrar);
        }


        interface IFutureRegistrar
        {
            IFutureRegistration Register(IServiceCollection collection, IContainerRegistrar registrar);
        }


        class FutureRegistrar<TFuture> :
            IFutureRegistrar
            where TFuture : class, SagaStateMachine<FutureState>
        {
            public virtual IFutureRegistration Register(IServiceCollection collection, IContainerRegistrar registrar)
            {
                collection.TryAddSingleton<TFuture>();

                return registrar.GetOrAddRegistration<IFutureRegistration>(typeof(TFuture), _ => new FutureRegistration<TFuture>(registrar));
            }
        }


        class FutureDefinitionRegistrar<TFuture, TDefinition> :
            FutureRegistrar<TFuture>
            where TDefinition : class, IFutureDefinition<TFuture>
            where TFuture : class, SagaStateMachine<FutureState>
        {
            public override IFutureRegistration Register(IServiceCollection collection, IContainerRegistrar registrar)
            {
                var registration = base.Register(collection, registrar);

                registrar.AddDefinition<IFutureDefinition<TFuture>, TDefinition>();

                return registration;
            }
        }
    }
}
