namespace MassTransit.SimpleInjectorIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SimpleInjector;

    using MassTransit.ConsumeConfigurators;
    using MassTransit.Internals.Extensions;
    using MassTransit.Saga;
    using MassTransit.Saga.SubscriptionConfigurators;
    
    public static class SimpleInjectorExtensions
    {
        /// <summary>
        /// Specify that the service bus should load its subscribers from the container passed as an argument.
        /// </summary>
        /// <param name="configurator">The configurator the extension method works on.</param>
        /// <param name="container">The SimpleInjector container.</param>
        /// <remarks>You should register your message consumers with AsyncScoped lifestyle.</remarks>
        public static void LoadFrom(this IReceiveEndpointConfigurator configurator, Container container)
        {
            IList<Type> concreteTypes = FindTypes<IConsumer>(container, x => !x.HasInterface<ISaga>());
            if (concreteTypes.Count > 0)
            {
                foreach (Type concreteType in concreteTypes)
                    ConsumerConfiguratorCache.Configure(concreteType, configurator, container);
            }

            IList<Type> sagaTypes = FindTypes<ISaga>(container, x => true);
            if (sagaTypes.Count > 0)
            {
                foreach (Type sagaType in sagaTypes)
                    SagaConfiguratorCache.Configure(sagaType, configurator, container);
            }
        }

        public static void Consumer<T>(this IReceiveEndpointConfigurator configurator, Container container,
                                       Action<IConsumerConfigurator<T>> configure = null)
            where T : class, IConsumer
        {
            var consumerFactory = new SimpleInjectorConsumerFactory<T>(container);

            configurator.Consumer(consumerFactory, configure);
        }

        public static void Saga<T>(this IReceiveEndpointConfigurator configurator, Container container,
                                   Action<ISagaConfigurator<T>> configure = null)
            where T : class, ISaga
        {
            var sagaRepository = container.GetInstance<ISagaRepository<T>>();

            var simpleInjectorSagaRepository = new SimpleInjectorSagaRepository<T>(sagaRepository, container);

            configurator.Saga(simpleInjectorSagaRepository, configure);
        }

        static IList<Type> FindTypes<T>(Container container, Func<Type, bool> filter)
        {
            return
                container.GetCurrentRegistrations()
                    .Where(r => r.Registration.ImplementationType.HasInterface<T>())
                    .Select(x => x.Registration.ImplementationType)
                    .Where(filter)
                    .ToList();
        }
    }
}