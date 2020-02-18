namespace MassTransit
{
    using System;
    using Automatonymous;
    using ConsumeConfigurators;
    using Courier;
    using ExtensionsDependencyInjectionIntegration.Registration;
    using ExtensionsDependencyInjectionIntegration.ScopeProviders;
    using GreenPipes;
    using Microsoft.Extensions.DependencyInjection;
    using Pipeline;
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
        /// Connect a consumer with a consumer factory method
        /// </summary>
        /// <typeparam name="TConsumer"></typeparam>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="provider"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static void Consumer<TConsumer, TMessage>(this IBatchConfigurator<TMessage> configurator, IServiceProvider provider,
            Action<IConsumerMessageConfigurator<TConsumer, Batch<TMessage>>> configure = null)
            where TConsumer : class, IConsumer<Batch<TMessage>>
            where TMessage : class
        {
            IConsumerScopeProvider scopeProvider = new DependencyInjectionConsumerScopeProvider(provider);

            IConsumerFactory<TConsumer> consumerFactory = new ScopeConsumerFactory<TConsumer>(scopeProvider);

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
            ISagaRepository<T> repository = CreateSagaRepository<T>(provider);

            configurator.Saga(repository, configure);
        }

        /// <summary>
        /// Subscribe a state machine saga to the endpoint
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <param name="configurator"></param>
        /// <param name="stateMachine"></param>
        /// <param name="serviceProvider">The Container reference to resolve the repository</param>
        /// <param name="configure">Optionally configure the saga</param>
        /// <returns></returns>
        public static void StateMachineSaga<TInstance>(this IReceiveEndpointConfigurator configurator, SagaStateMachine<TInstance> stateMachine,
            IServiceProvider serviceProvider, Action<ISagaConfigurator<TInstance>> configure = null)
            where TInstance : class, SagaStateMachineInstance
        {
            ISagaRepository<TInstance> repository = CreateSagaRepository<TInstance>(serviceProvider);

            configurator.StateMachineSaga(stateMachine, repository, configure);
        }

        /// <summary>
        /// Subscribe a state machine saga to the endpoint
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <param name="configurator"></param>
        /// <param name="provider">The Container reference to resolve the repository</param>
        /// <param name="configure">Optionally configure the saga</param>
        /// <returns></returns>
        public static void StateMachineSaga<TInstance>(this IReceiveEndpointConfigurator configurator, IServiceProvider provider,
            Action<ISagaConfigurator<TInstance>> configure = null)
            where TInstance : class, SagaStateMachineInstance
        {
            var stateMachine = ResolveSagaStateMachine<TInstance>(provider);

            ISagaRepository<TInstance> repository = CreateSagaRepository<TInstance>(provider);

            configurator.StateMachineSaga(stateMachine, repository, configure);
        }

        public static ConnectHandle ConnectStateMachineSaga<TInstance>(this IConsumePipeConnector bus, SagaStateMachine<TInstance> stateMachine,
            IServiceProvider provider, Action<ISagaConfigurator<TInstance>> configure = null)
            where TInstance : class, SagaStateMachineInstance
        {
            ISagaRepository<TInstance> repository = CreateSagaRepository<TInstance>(provider);

            return bus.ConnectStateMachineSaga(stateMachine, repository, configure);
        }

        public static ConnectHandle ConnectStateMachineSaga<TInstance>(this IConsumePipeConnector bus, IServiceProvider provider,
            Action<ISagaConfigurator<TInstance>> configure = null)
            where TInstance : class, SagaStateMachineInstance
        {
            var stateMachine = ResolveSagaStateMachine<TInstance>(provider);

            ISagaRepository<TInstance> repository = CreateSagaRepository<TInstance>(provider);

            return bus.ConnectStateMachineSaga(stateMachine, repository, configure);
        }

        static ISagaRepository<TInstance> CreateSagaRepository<TInstance>(IServiceProvider provider)
            where TInstance : class, ISaga
        {
            ISagaRepositoryFactory repositoryFactory = new DependencyInjectionSagaRepositoryFactory(provider);

            return repositoryFactory.CreateSagaRepository<TInstance>();
        }

        static SagaStateMachine<TInstance> ResolveSagaStateMachine<TInstance>(IServiceProvider provider)
            where TInstance : class, SagaStateMachineInstance
        {
            return provider.GetRequiredService<SagaStateMachine<TInstance>>();
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
