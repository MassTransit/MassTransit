namespace MassTransit
{
    using System;
    using ConsumeConfigurators;
    using Courier;
    using Lamar;
    using LamarIntegration.Registration;
    using LamarIntegration.ScopeProviders;
    using Registration;
    using Saga;
    using Scoping;


    public static class LamarReceiveEndpointExtensions
    {
        /// <summary>
        /// Registers a consumer given the lifetime scope specified
        /// </summary>
        /// <typeparam name="T">The consumer type</typeparam>
        /// <param name="configurator">The service bus configurator</param>
        /// <param name="context">The LifetimeScope of the container</param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static void Consumer<T>(this IReceiveEndpointConfigurator configurator, IServiceContext context,
            Action<IConsumerConfigurator<T>> configure = null)
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
            IConsumerScopeProvider scopeProvider = new LamarConsumerScopeProvider(container);

            var consumerFactory = new ScopeConsumerFactory<T>(scopeProvider);

            configurator.Consumer(consumerFactory, configure);
        }

        /// <summary>
        /// Registers a saga using the container that has the repository resolved from the container
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="context"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static void Saga<T>(this IReceiveEndpointConfigurator configurator, IServiceContext context, Action<ISagaConfigurator<T>> configure = null)
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
            ISagaRepositoryFactory factory = new LamarSagaRepositoryFactory(container);

            ISagaRepository<T> sagaRepository = factory.CreateSagaRepository<T>();

            configurator.Saga(sagaRepository, configure);
        }

        public static void ExecuteActivityHost<TActivity, TArguments>(this IReceiveEndpointConfigurator configurator, Uri compensateAddress,
            IServiceContext context, Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure = null)
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
            var executeActivityScopeProvider = new LamarExecuteActivityScopeProvider<TActivity, TArguments>(container);

            var factory = new ScopeExecuteActivityFactory<TActivity, TArguments>(executeActivityScopeProvider);

            configurator.ExecuteActivityHost(compensateAddress, factory, configure);
        }

        public static void ExecuteActivityHost<TActivity, TArguments>(this IReceiveEndpointConfigurator configurator,
            IServiceContext context, Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure = null)
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
            var executeActivityScopeProvider = new LamarExecuteActivityScopeProvider<TActivity, TArguments>(container);

            var factory = new ScopeExecuteActivityFactory<TActivity, TArguments>(executeActivityScopeProvider);

            configurator.ExecuteActivityHost(factory, configure);
        }

        public static void CompensateActivityHost<TActivity, TLog>(this IReceiveEndpointConfigurator configurator, IServiceContext context,
            Action<ICompensateActivityConfigurator<TActivity, TLog>> configure = null)
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
            var compensateActivityScopeProvider = new LamarCompensateActivityScopeProvider<TActivity, TLog>(container);

            var factory = new ScopeCompensateActivityFactory<TActivity, TLog>(compensateActivityScopeProvider);

            configurator.CompensateActivityHost(factory, configure);
        }
    }
}
