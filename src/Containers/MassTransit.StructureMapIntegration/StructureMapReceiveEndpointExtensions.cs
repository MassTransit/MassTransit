namespace MassTransit
{
    using System;
    using ConsumeConfigurators;
    using Courier;
    using Registration;
    using Saga;
    using Scoping;
    using StructureMap;
    using StructureMapIntegration.Registration;
    using StructureMapIntegration.ScopeProviders;


    public static class StructureMapReceiveEndpointExtensions
    {
        /// <summary>
        /// Registers a consumer given the lifetime scope specified
        /// </summary>
        /// <typeparam name="T">The consumer type</typeparam>
        /// <param name="configurator">The service bus configurator</param>
        /// <param name="context">The LifetimeScope of the container</param>
        /// <returns></returns>
        public static void Consumer<T>(this IReceiveEndpointConfigurator configurator, IContext context)
            where T : class, IConsumer
        {
            Consumer<T>(configurator, context.GetInstance<IContainer>());
        }

        /// <summary>
        /// Registers a consumer given the lifetime scope specified
        /// </summary>
        /// <typeparam name="T">The consumer type</typeparam>
        /// <param name="configurator">The service bus configurator</param>
        /// <param name="context">The LifetimeScope of the container</param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static void Consumer<T>(this IReceiveEndpointConfigurator configurator, IContext context, Action<IConsumerConfigurator<T>> configure)
            where T : class, IConsumer
        {
            Consumer(configurator, context.GetInstance<IContainer>(), configure);
        }

        /// <summary>
        /// Registers a consumer given the lifetime scope specified
        /// </summary>
        /// <typeparam name="T">The consumer type</typeparam>
        /// <param name="configurator">The service bus configurator</param>
        /// <param name="container">The LifetimeScope of the container</param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static void Consumer<T>(this IReceiveEndpointConfigurator configurator, IContainer container, Action<IConsumerConfigurator<T>> configure = null)
            where T : class, IConsumer
        {
            IConsumerScopeProvider scopeProvider = new StructureMapConsumerScopeProvider(container);

            var consumerFactory = new ScopeConsumerFactory<T>(scopeProvider);

            configurator.Consumer(consumerFactory, configure);
        }

        /// <summary>
        /// Registers a saga using the container that has the repository resolved from the container
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static void Saga<T>(this IReceiveEndpointConfigurator configurator, IContext context)
            where T : class, ISaga
        {
            Saga<T>(configurator, context.GetInstance<IContainer>());
        }

        /// <summary>
        /// Registers a saga using the container that has the repository resolved from the container
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="context"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static void Saga<T>(this IReceiveEndpointConfigurator configurator, IContext context, Action<ISagaConfigurator<T>> configure)
            where T : class, ISaga
        {
            Saga(configurator, context.GetInstance<IContainer>(), configure);
        }

        /// <summary>
        /// Registers a saga using the container that has the repository resolved from the container
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="container"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static void Saga<T>(this IReceiveEndpointConfigurator configurator, IContainer container, Action<ISagaConfigurator<T>> configure = null)
            where T : class, ISaga
        {
            ISagaRepositoryFactory factory = new StructureMapSagaRepositoryFactory(container);

            ISagaRepository<T> sagaRepository = factory.CreateSagaRepository<T>();

            configurator.Saga(sagaRepository, configure);
        }

        public static void ExecuteActivityHost<TActivity, TArguments>(this IReceiveEndpointConfigurator configurator, Uri compensateAddress,
            IContext context)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            ExecuteActivityHost<TActivity, TArguments>(configurator, compensateAddress, context.GetInstance<IContainer>());
        }

        public static void ExecuteActivityHost<TActivity, TArguments>(this IReceiveEndpointConfigurator configurator, Uri compensateAddress,
            IContext context, Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            ExecuteActivityHost(configurator, compensateAddress, context.GetInstance<IContainer>(), configure);
        }

        public static void ExecuteActivityHost<TActivity, TArguments>(this IReceiveEndpointConfigurator configurator, Uri compensateAddress,
            IContainer container, Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure = null)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            var executeActivityScopeProvider = new StructureMapExecuteActivityScopeProvider<TActivity, TArguments>(container);

            var factory = new ScopeExecuteActivityFactory<TActivity, TArguments>(executeActivityScopeProvider);

            configurator.ExecuteActivityHost(compensateAddress, factory, configure);
        }

        public static void ExecuteActivityHost<TActivity, TArguments>(this IReceiveEndpointConfigurator configurator,
            IContext context)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            ExecuteActivityHost<TActivity, TArguments>(configurator, context.GetInstance<IContainer>());
        }

        public static void ExecuteActivityHost<TActivity, TArguments>(this IReceiveEndpointConfigurator configurator,
            IContext context, Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            ExecuteActivityHost(configurator, context.GetInstance<IContainer>(), configure);
        }

        public static void ExecuteActivityHost<TActivity, TArguments>(this IReceiveEndpointConfigurator configurator, IContainer container,
            Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure = null)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            var executeActivityScopeProvider = new StructureMapExecuteActivityScopeProvider<TActivity, TArguments>(container);

            var factory = new ScopeExecuteActivityFactory<TActivity, TArguments>(executeActivityScopeProvider);

            configurator.ExecuteActivityHost(factory, configure);
        }

        public static void CompensateActivityHost<TActivity, TLog>(this IReceiveEndpointConfigurator configurator, IContext context)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            CompensateActivityHost<TActivity, TLog>(configurator, context.GetInstance<IContainer>());
        }

        public static void CompensateActivityHost<TActivity, TLog>(this IReceiveEndpointConfigurator configurator, IContext context,
            Action<ICompensateActivityConfigurator<TActivity, TLog>> configure)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            CompensateActivityHost(configurator, context.GetInstance<IContainer>(), configure);
        }

        public static void CompensateActivityHost<TActivity, TLog>(this IReceiveEndpointConfigurator configurator, IContainer container,
            Action<ICompensateActivityConfigurator<TActivity, TLog>> configure = null)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            var compensateActivityScopeProvider = new StructureMapCompensateActivityScopeProvider<TActivity, TLog>(container);

            var factory = new ScopeCompensateActivityFactory<TActivity, TLog>(compensateActivityScopeProvider);

            configurator.CompensateActivityHost(factory, configure);
        }
    }
}
