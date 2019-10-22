namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Internals.Extensions;
    using Registration;
    using Saga;
    using StructureMap;
    using StructureMapIntegration.Registration;
    using StructureMapIntegration.ScopeProviders;


    public static class StructureMapExtensions
    {
        /// <summary>
        /// Specify that the service bus should load its subscribers from the container passed as an argument.
        /// </summary>
        /// <param name="configurator">The configurator the extension method works on.</param>
        /// <param name="container">The StructureMap container.</param>
        [Obsolete("LoadFrom is not recommended, review the documentation and use the Consumer methods for your container instead.")]
        public static void LoadFrom(this IReceiveEndpointConfigurator configurator, IContainer container)
        {
            var scopeProvider = new StructureMapConsumerScopeProvider(container);

            IList<Type> concreteTypes = FindTypes<IConsumer>(container, x => !x.HasInterface<ISaga>());
            if (concreteTypes.Count > 0)
            {
                foreach (var concreteType in concreteTypes)
                    ConsumerConfiguratorCache.Configure(concreteType, configurator, scopeProvider);
            }

            var sagaRepositoryFactory = new StructureMapSagaRepositoryFactory(container);

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
        [Obsolete("LoadFrom is not recommended, review the documentation and use the Consumer methods for your container instead.")]
        public static void LoadFrom(this IReceiveEndpointConfigurator configurator, IContext context)
        {
            var container = context.GetInstance<IContainer>();

            configurator.LoadFrom(container);
        }

        /// <summary>
        /// Registers the InMemory saga repository for all saga types (generic, can be overridden)
        /// </summary>
        /// <param name="registry"></param>
        public static void RegisterInMemorySagaRepository(this ConfigurationExpression registry)
        {
            registry.For(typeof(ISagaRepository<>))
                .Use(typeof(InMemorySagaRepository<>))
                .Singleton();
        }

        /// <summary>
        /// Register the InMemory saga repository for the specified saga type
        /// </summary>
        /// <param name="registry"></param>
        /// <typeparam name="T"></typeparam>
        public static void RegisterInMemorySagaRepository<T>(this ConfigurationExpression registry)
            where T : class, ISaga
        {
            registry.For<ISagaRepository<T>>()
                .Use<InMemorySagaRepository<T>>()
                .Singleton();
        }

        static IList<Type> FindTypes<T>(IContainer container, Func<Type, bool> filter)
        {
            return container
                .Model
                .PluginTypes
                .Where(x => x.PluginType.HasInterface<T>())
                .Select(i => i.PluginType)
                .Concat(container.Model.InstancesOf<T>().Select(x => x.ReturnedType))
                .Where(filter)
                .Distinct()
                .ToList();
        }

        internal static IContainer CreateNestedContainer(this IContainer container, ConsumeContext context)
        {
            var nestedContainer = container.GetNestedContainer();
            nestedContainer.Configure(x =>
            {
                x.For<ConsumeContext>()
                    .Use(context);
            });

            return nestedContainer;
        }

        internal static IContainer CreateNestedContainer(this IContext container, ConsumeContext context)
        {
            var nestedContainer = container.GetInstance<IContainer>().GetNestedContainer();
            nestedContainer.Configure(x =>
            {
                x.For<ConsumeContext>()
                    .Use(context);
            });

            return nestedContainer;
        }

        internal static IContainer CreateNestedContainer<T>(this IContainer container, ConsumeContext<T> context)
            where T : class
        {
            var nestedContainer = container.GetNestedContainer();
            nestedContainer.Configure(x =>
            {
                x.For<ConsumeContext>()
                    .Use(context);

                x.For<ConsumeContext<T>>()
                    .Use(context);
            });

            return nestedContainer;
        }

        internal static IContainer CreateNestedContainer<T>(this IContext container, ConsumeContext<T> context)
            where T : class
        {
            var nestedContainer = container.GetInstance<IContainer>().GetNestedContainer();
            nestedContainer.Configure(x =>
            {
                x.For<ConsumeContext>()
                    .Use(context);

                x.For<ConsumeContext<T>>()
                    .Use(context);
            });

            return nestedContainer;
        }
    }
}
