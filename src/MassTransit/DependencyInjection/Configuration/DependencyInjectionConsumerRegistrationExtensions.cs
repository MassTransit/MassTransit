namespace MassTransit.Configuration
{
    using System;
    using DependencyInjection.Registration;
    using Internals;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;


    public static class DependencyInjectionConsumerRegistrationExtensions
    {
        public static IConsumerRegistration RegisterConsumer<T>(this IServiceCollection collection)
            where T : class, IConsumer
        {
            return RegisterConsumer<T>(collection, new DependencyInjectionContainerRegistrar(collection));
        }

        public static IConsumerRegistration RegisterConsumer<T>(this IServiceCollection collection, IContainerRegistrar registrar)
            where T : class, IConsumer
        {
            if (MessageTypeCache<T>.HasSagaInterfaces)
                throw new ArgumentException($"{TypeCache<T>.ShortName} is a saga, and cannot be registered as a consumer", nameof(T));

            return new ConsumerRegistrar<T>().Register(collection, registrar);
        }

        public static IConsumerRegistration RegisterConsumer<T, TDefinition>(this IServiceCollection collection)
            where T : class, IConsumer
            where TDefinition : class, IConsumerDefinition<T>
        {
            return RegisterConsumer<T, TDefinition>(collection, new DependencyInjectionContainerRegistrar(collection));
        }

        public static IConsumerRegistration RegisterConsumer<T, TDefinition>(this IServiceCollection collection, IContainerRegistrar registrar)
            where T : class, IConsumer
            where TDefinition : class, IConsumerDefinition<T>
        {
            if (MessageTypeCache<T>.HasSagaInterfaces)
                throw new ArgumentException($"{TypeCache<T>.ShortName} is a saga, and cannot be registered as a consumer", nameof(T));

            return new ConsumerDefinitionRegistrar<T, TDefinition>().Register(collection, registrar);
        }

        public static IConsumerRegistration RegisterConsumer<T>(this IServiceCollection collection, Type consumerDefinitionType)
            where T : class, IConsumer
        {
            return RegisterConsumer<T>(collection, new DependencyInjectionContainerRegistrar(collection), consumerDefinitionType);
        }

        public static IConsumerRegistration RegisterConsumer<T>(this IServiceCollection collection, IContainerRegistrar registrar, Type consumerDefinitionType)
            where T : class, IConsumer
        {
            if (consumerDefinitionType == null)
                return RegisterConsumer<T>(collection, registrar);

            if (MessageTypeCache<T>.HasSagaInterfaces)
                throw new ArgumentException($"{TypeCache<T>.ShortName} is a saga, and cannot be registered as a consumer", nameof(T));

            if (!consumerDefinitionType.ClosesType(typeof(IConsumerDefinition<>), out Type[] types) || types[0] != typeof(T))
            {
                throw new ArgumentException($"{TypeCache.GetShortName(consumerDefinitionType)} is not a consumer definition of {TypeCache<T>.ShortName}",
                    nameof(consumerDefinitionType));
            }

            var register = (IConsumerRegistrar)Activator.CreateInstance(
                typeof(ConsumerDefinitionRegistrar<,>).MakeGenericType(typeof(T), consumerDefinitionType));

            return register.Register(collection, registrar);
        }


        interface IConsumerRegistrar
        {
            IConsumerRegistration Register(IServiceCollection collection, IContainerRegistrar registrar);
        }


        class ConsumerRegistrar<TConsumer> :
            IConsumerRegistrar
            where TConsumer : class, IConsumer
        {
            public virtual IConsumerRegistration Register(IServiceCollection collection, IContainerRegistrar registrar)
            {
                collection.TryAddScoped<TConsumer>();

                return registrar.GetOrAdd<IConsumerRegistration>(typeof(TConsumer), _ => new ConsumerRegistration<TConsumer>());
            }
        }


        class ConsumerDefinitionRegistrar<TConsumer, TDefinition> :
            ConsumerRegistrar<TConsumer>
            where TDefinition : class, IConsumerDefinition<TConsumer>
            where TConsumer : class, IConsumer
        {
            public override IConsumerRegistration Register(IServiceCollection collection, IContainerRegistrar registrar)
            {
                var registration = base.Register(collection, registrar);

                collection.AddSingleton<TDefinition>();
                collection.AddSingleton<IConsumerDefinition<TConsumer>>(provider => provider.GetRequiredService<TDefinition>());

                return registration;
            }
        }
    }
}
