namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Internals.Extensions;
    using Lamar;
    using LamarIntegration.Registration;
    using LamarIntegration.ScopeProviders;
    using Microsoft.Extensions.DependencyInjection;
    using Registration;
    using Saga;


    public static class LamarExtensions
    {
        static Type[] FindTypes<T>(IServiceContext container, Func<Type, bool> filter)
        {
            IEnumerable<Type> serviceTypes = container.Model.ServiceTypes.Where(x => x.ServiceType.HasInterface<T>()).Select(x => x.ServiceType);
            IEnumerable<Type> serviceInstanceTypes =
                serviceTypes.SelectMany(serviceType => container.Model.InstancesOf(serviceType)).Select(x => x.ImplementationType);

            IEnumerable<Type> instanceTypes = container.Model.InstancesOf<T>().Select(x => x.ImplementationType);

            return serviceInstanceTypes
                .Concat(instanceTypes)
                .Where(filter)
                .Distinct()
                .ToArray();
        }

        /// <summary>
        /// Specify that the service bus should load its subscribers from the container passed as an argument.
        /// </summary>
        /// <param name="configurator">The configurator the extension method works on.</param>
        /// <param name="container">The Lamar container.</param>
        [Obsolete(
            "This method is not recommended, since it may load multiple consumers into a single receive endpoint. Review the documentation and use the Consumer methods for your container instead.")]
        public static void LoadFrom(this IReceiveEndpointConfigurator configurator, IContainer container)
        {
            var scopeProvider = new LamarConsumerScopeProvider(container);

            IList<Type> concreteTypes = FindTypes<IConsumer>(container, x => !x.HasInterface<ISaga>());
            if (concreteTypes.Count > 0)
            {
                foreach (var concreteType in concreteTypes)
                    ConsumerConfiguratorCache.Configure(concreteType, configurator, scopeProvider);
            }

            var sagaRepositoryFactory = new LamarSagaRepositoryFactory(container);

            IList<Type> sagaTypes = FindTypes<ISaga>(container, x => true);
            if (sagaTypes.Count > 0)
            {
                foreach (var sagaType in sagaTypes)
                    SagaConfiguratorCache.Configure(sagaType, configurator, sagaRepositoryFactory);
            }
        }

        /// <summary>
        /// Specify that the service bus should load its subscribers from the container passed as an argument.
        /// </summary>
        /// <param name="configurator">The configurator the extension method works on.</param>
        /// <param name="context"></param>
        [Obsolete(
            "This method is not recommended, since it may load multiple consumers into a single receive endpoint. Review the documentation and use the Consumer methods for your container instead.")]
        public static void LoadFrom(this IReceiveEndpointConfigurator configurator, IServiceContext context)
        {
            var container = context.GetInstance<IContainer>();

            configurator.LoadFrom(container);
        }

        internal static INestedContainer GetNestedContainer(this IContainer container, ConsumeContext context)
        {
            var nestedContainer = container.GetNestedContainer();
            nestedContainer.Inject(context);

            return nestedContainer;
        }

        /// <summary>
        /// Registers the InMemory saga repository for all saga types (generic, can be overridden)
        /// </summary>
        /// <param name="registry"></param>
        public static void RegisterInMemorySagaRepository(this ServiceRegistry registry)
        {
            registry.AddSingleton(typeof(ISagaRepository<>), typeof(InMemorySagaRepository<>));
        }

        /// <summary>
        /// Register the InMemory saga repository for the specified saga type
        /// </summary>
        /// <param name="registry"></param>
        /// <typeparam name="T"></typeparam>
        public static void RegisterInMemorySagaRepository<T>(this ServiceRegistry registry)
            where T : class, ISaga
        {
            registry.AddSingleton<ISagaRepository<T>, InMemorySagaRepository<T>>();
        }
    }
}
