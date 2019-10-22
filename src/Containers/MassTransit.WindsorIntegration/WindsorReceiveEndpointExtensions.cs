namespace MassTransit
{
    using System;
    using Castle.MicroKernel;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using ConsumeConfigurators;
    using Courier;
    using Registration;
    using Saga;
    using Scoping;
    using WindsorIntegration.Registration;
    using WindsorIntegration.ScopeProviders;


    public static class WindsorReceiveEndpointExtensions
    {
        /// <summary>
        /// Registers a consumer given the lifetime scope specified
        /// </summary>
        /// <typeparam name="T">The consumer type</typeparam>
        /// <param name="configurator">The service bus configurator</param>
        /// <param name="kernel">The LifetimeScope of the container</param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static void Consumer<T>(this IReceiveEndpointConfigurator configurator, IKernel kernel, Action<IConsumerConfigurator<T>> configure = null)
            where T : class, IConsumer
        {
            IConsumerScopeProvider scopeProvider = new WindsorConsumerScopeProvider(kernel);

            var consumerFactory = new ScopeConsumerFactory<T>(scopeProvider);

            configurator.Consumer(consumerFactory, configure);
        }

        /// <summary>
        /// Registers a consumer given the lifetime scope specified
        /// </summary>
        /// <typeparam name="T">The consumer type</typeparam>
        /// <param name="configurator">The service bus configurator</param>
        /// <param name="container">The LifetimeScope of the container</param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static void Consumer<T>(this IReceiveEndpointConfigurator configurator, IWindsorContainer container, Action<IConsumerConfigurator<T>>
            configure = null)
            where T : class, IConsumer
        {
            RegisterScopedContextProviderIfNotPresent(container);

            Consumer(configurator, container.Kernel, configure);
        }

        /// <summary>
        /// Registers a saga using the container that has the repository resolved from the container
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="kernel"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static void Saga<T>(this IReceiveEndpointConfigurator configurator, IKernel kernel, Action<ISagaConfigurator<T>> configure = null)
            where T : class, ISaga
        {
            ISagaRepositoryFactory factory = new WindsorSagaRepositoryFactory(kernel);

            ISagaRepository<T> sagaRepository = factory.CreateSagaRepository<T>();

            configurator.Saga(sagaRepository, configure);
        }

        /// <summary>
        /// Registers a saga using the container that has the repository resolved from the container
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="container"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static void Saga<T>(this IReceiveEndpointConfigurator configurator, IWindsorContainer container, Action<ISagaConfigurator<T>> configure = null)
            where T : class, ISaga
        {
            RegisterScopedContextProviderIfNotPresent(container);

            Saga(configurator, container.Kernel, configure);
        }

        public static void ExecuteActivityHost<TActivity, TArguments>(this IReceiveEndpointConfigurator configurator, Uri compensateAddress, IKernel kernel,
            Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure = null)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            var executeActivityScopeProvider = new WindsorExecuteActivityScopeProvider<TActivity, TArguments>(kernel);

            var factory = new ScopeExecuteActivityFactory<TActivity, TArguments>(executeActivityScopeProvider);

            configurator.ExecuteActivityHost(compensateAddress, factory, configure);
        }

        public static void ExecuteActivityHost<TActivity, TArguments>(this IReceiveEndpointConfigurator configurator, Uri compensateAddress,
            IWindsorContainer container, Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure = null)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            RegisterScopedContextProviderIfNotPresent(container);

            ExecuteActivityHost(configurator, compensateAddress, container.Kernel, configure);
        }

        public static void ExecuteActivityHost<TActivity, TArguments>(this IReceiveEndpointConfigurator configurator, IKernel kernel,
            Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure = null)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            var executeActivityScopeProvider = new WindsorExecuteActivityScopeProvider<TActivity, TArguments>(kernel);

            var factory = new ScopeExecuteActivityFactory<TActivity, TArguments>(executeActivityScopeProvider);

            configurator.ExecuteActivityHost(factory, configure);
        }

        public static void ExecuteActivityHost<TActivity, TArguments>(this IReceiveEndpointConfigurator configurator, IWindsorContainer container,
            Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure = null)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            RegisterScopedContextProviderIfNotPresent(container);

            ExecuteActivityHost(configurator, container.Kernel, configure);
        }

        public static void CompensateActivityHost<TActivity, TLog>(this IReceiveEndpointConfigurator configurator, IKernel kernel,
            Action<ICompensateActivityConfigurator<TActivity, TLog>> configure = null)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            var compensateActivityScopeProvider = new WindsorCompensateActivityScopeProvider<TActivity, TLog>(kernel);

            var factory = new ScopeCompensateActivityFactory<TActivity, TLog>(compensateActivityScopeProvider);

            configurator.CompensateActivityHost(factory, configure);
        }

        public static void CompensateActivityHost<TActivity, TLog>(this IReceiveEndpointConfigurator configurator, IWindsorContainer container,
            Action<ICompensateActivityConfigurator<TActivity, TLog>> configure = null)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            RegisterScopedContextProviderIfNotPresent(container);

            CompensateActivityHost(configurator, container.Kernel, configure);
        }

        static void RegisterScopedContextProviderIfNotPresent(IWindsorContainer container)
        {
            if (!container.Kernel.HasComponent(typeof(ScopedConsumeContextProvider)))
            {
                container.Register(Component.For<ScopedConsumeContextProvider>().LifestyleScoped(),
                    Component.For<ConsumeContext>().UsingFactoryMethod(kernel => kernel.Resolve<ScopedConsumeContextProvider>().GetContext())
                        .LifestyleScoped());
            }
        }
    }
}
