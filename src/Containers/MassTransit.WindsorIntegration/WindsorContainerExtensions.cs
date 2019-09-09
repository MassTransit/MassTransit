namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Castle.MicroKernel;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Internals.Extensions;
    using Registration;
    using Saga;
    using WindsorIntegration.Configuration;
    using WindsorIntegration.Registration;
    using WindsorIntegration.ScopeProviders;


    /// <summary>
    /// Extension methods for the Windsor IoC container.
    /// </summary>
    public static class WindsorContainerExtensions
    {
        /// <summary>
        /// Specify that the service bus should load its subscribers from the container passed as an argument.
        /// </summary>
        /// <param name="configurator">The configurator the extension method works on.</param>
        /// <param name="container">The Windsor container.</param>
        [Obsolete("LoadFrom is not recommended, review the documentation and use the Consumer methods for your container instead.")]
        public static void LoadFrom(this IReceiveEndpointConfigurator configurator, IWindsorContainer container)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            LoadFrom(configurator, container.Kernel);
        }

        /// <summary>
        /// Specify that the service bus should load its subscribers from the container passed as an argument.
        /// </summary>
        /// <param name="configurator">The configurator the extension method works on.</param>
        /// <param name="kernel">The Windsor container.</param>
        [Obsolete("LoadFrom is not recommended, review the documentation and use the Consumer methods for your container instead.")]
        public static void LoadFrom(this IReceiveEndpointConfigurator configurator, IKernel kernel)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            if (kernel == null)
                throw new ArgumentNullException(nameof(kernel));

            IList<Type> consumerTypes = FindTypes<IConsumer>(kernel, x => !x.HasInterface<ISaga>());
            if (consumerTypes.Count > 0)
            {
                var scopeProvider = new WindsorConsumerScopeProvider(kernel);

                foreach (var type in consumerTypes)
                    ConsumerConfiguratorCache.Configure(type, configurator, scopeProvider);
            }

            IList<Type> sagaTypes = FindTypes<ISaga>(kernel, x => true);
            if (sagaTypes.Count > 0)
            {
                var repositoryFactory = new WindsorSagaRepositoryFactory(kernel);

                foreach (var sagaType in sagaTypes)
                    SagaConfiguratorCache.Configure(sagaType, configurator, repositoryFactory);
            }
        }

        /// <summary>
        /// Registers the InMemory saga repository for all saga types (generic, can be overridden)
        /// </summary>
        /// <param name="container"></param>
        public static void RegisterInMemorySagaRepository(this IWindsorContainer container)
        {
            container.Register(Component.For(typeof(ISagaRepository<>)).ImplementedBy(typeof(InMemorySagaRepository<>)).LifestyleSingleton());
        }

        /// <summary>
        /// Register the InMemory saga repository for the specified saga type
        /// </summary>
        /// <param name="container"></param>
        /// <typeparam name="T"></typeparam>
        public static void RegisterInMemorySagaRepository<T>(this IWindsorContainer container)
            where T : class, ISaga
        {
            container.Register(Component.For<ISagaRepository<T>>().ImplementedBy<InMemorySagaRepository<T>>().LifestyleSingleton());
        }

        /// <summary>
        /// Enables message scope lifetime for windsor containers
        /// </summary>
        /// <param name="configurator"></param>
        [Obsolete("Change to UseMessageScope - method was renamed to match middleware conventions")]
        public static void EnableMessageScope(this IConsumePipeConfigurator configurator)
        {
            UseMessageScope(configurator);
        }

        /// <summary>
        /// Enables message scope lifetime for windsor containers
        /// </summary>
        /// <param name="configurator"></param>
        public static void UseMessageScope(this IConsumePipeConfigurator configurator)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var specification = new WindsorMessageScopePipeSpecification();

            configurator.AddPrePipeSpecification(specification);
        }

        static IList<Type> FindTypes<T>(IKernel container, Func<Type, bool> filter)
        {
            return container
                .GetAssignableHandlers(typeof(T))
                .Select(h => h.ComponentModel.Services.First())
                .Where(filter)
                .ToList();
        }
    }
}
