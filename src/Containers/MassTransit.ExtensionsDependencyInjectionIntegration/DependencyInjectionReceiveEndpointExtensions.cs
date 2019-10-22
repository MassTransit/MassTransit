namespace MassTransit
{
    using System;
    using ConsumeConfigurators;
    using Courier;
    using ExtensionsDependencyInjectionIntegration.Registration;
    using ExtensionsDependencyInjectionIntegration.ScopeProviders;
    using Registration;
    using Saga;
    using Scoping;


    public static class DependencyInjectionReceiveEndpointExtensions
    {
        /// <summary>
        /// Registers a consumer given the lifetime scope specified
        /// </summary>
        /// <typeparam name="T">The consumer type</typeparam>
        /// <param name="configurator">The service bus configurator</param>
        /// <param name="provider">The LifetimeScope of the provider</param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static void Consumer<T>(this IReceiveEndpointConfigurator configurator, IServiceProvider provider,
            Action<IConsumerConfigurator<T>> configure = null)
            where T : class, IConsumer
        {
            IConsumerScopeProvider scopeProvider = new DependencyInjectionConsumerScopeProvider(provider);

            var consumerFactory = new ScopeConsumerFactory<T>(scopeProvider);

            configurator.Consumer(consumerFactory, configure);
        }

        /// <summary>
        /// Registers a saga using the container that has the repository resolved from the container
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="provider"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static void Saga<T>(this IReceiveEndpointConfigurator configurator, IServiceProvider provider, Action<ISagaConfigurator<T>> configure = null)
            where T : class, ISaga
        {
            ISagaRepositoryFactory factory = new DependencyInjectionSagaRepositoryFactory(provider);

            ISagaRepository<T> sagaRepository = factory.CreateSagaRepository<T>();

            configurator.Saga(sagaRepository, configure);
        }

        public static void ExecuteActivityHost<TActivity, TArguments>(this IReceiveEndpointConfigurator configurator, Uri compensateAddress,
            IServiceProvider provider, Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure = null)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            var executeActivityScopeProvider = new DependencyInjectionExecuteActivityScopeProvider<TActivity, TArguments>(provider);

            var factory = new ScopeExecuteActivityFactory<TActivity, TArguments>(executeActivityScopeProvider);

            configurator.ExecuteActivityHost(compensateAddress, factory, configure);
        }

        public static void ExecuteActivityHost<TActivity, TArguments>(this IReceiveEndpointConfigurator configurator, IServiceProvider provider,
            Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure = null)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            var executeActivityScopeProvider = new DependencyInjectionExecuteActivityScopeProvider<TActivity, TArguments>(provider);

            var factory = new ScopeExecuteActivityFactory<TActivity, TArguments>(executeActivityScopeProvider);

            configurator.ExecuteActivityHost(factory, configure);
        }

        public static void CompensateActivityHost<TActivity, TLog>(this IReceiveEndpointConfigurator configurator, IServiceProvider provider,
            Action<ICompensateActivityConfigurator<TActivity, TLog>> configure = null)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            var compensateActivityScopeProvider = new DependencyInjectionCompensateActivityScopeProvider<TActivity, TLog>(provider);

            var factory = new ScopeCompensateActivityFactory<TActivity, TLog>(compensateActivityScopeProvider);

            configurator.CompensateActivityHost(factory, configure);
        }
    }
}
