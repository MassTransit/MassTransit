namespace MassTransit
{
    using System;
    using Automatonymous;
    using Automatonymous.SagaConfigurators;
    using Automatonymous.StateMachineConnectors;
    using Castle.MicroKernel;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using ConsumeConfigurators;
    using Context;
    using Courier;
    using GreenPipes;
    using Pipeline;
    using Registration;
    using Saga;
    using Scoping;
    using WindsorIntegration.Configuration;
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
        /// Connect a consumer with a consumer factory method
        /// </summary>
        /// <typeparam name="TConsumer"></typeparam>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="kernel"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static void Consumer<TConsumer, TMessage>(this IBatchConfigurator<TMessage> configurator, IKernel kernel,
            Action<IConsumerMessageConfigurator<TConsumer, Batch<TMessage>>> configure = null)
            where TConsumer : class, IConsumer<Batch<TMessage>>
            where TMessage : class
        {
            IConsumerScopeProvider scopeProvider = new WindsorConsumerScopeProvider(kernel);

            IConsumerFactory<TConsumer> consumerFactory = new ScopeConsumerFactory<TConsumer>(scopeProvider);

            configurator.Consumer(consumerFactory, configure);
        }

        /// <summary>
        /// Connect a consumer with a consumer factory method
        /// </summary>
        /// <typeparam name="TConsumer"></typeparam>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="container"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static void Consumer<TConsumer, TMessage>(this IBatchConfigurator<TMessage> configurator, IWindsorContainer container,
            Action<IConsumerMessageConfigurator<TConsumer, Batch<TMessage>>> configure = null)
            where TConsumer : class, IConsumer<Batch<TMessage>>
            where TMessage : class
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
            ISagaRepository<T> repository = ResolveSagaRepository<T>(kernel);

            configurator.Saga(repository, configure);
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

        /// <summary>
        /// Subscribe a state machine saga to the endpoint
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <param name="configurator"></param>
        /// <param name="stateMachine">The state machine</param>
        /// <param name="kernel">The Windsor Lifetime container to resolve the repository</param>
        /// <param name="configure">Optionally configure the saga</param>
        /// <returns></returns>
        public static void StateMachineSaga<TInstance>(this IReceiveEndpointConfigurator configurator, SagaStateMachine<TInstance> stateMachine,
            IKernel kernel, Action<ISagaConfigurator<TInstance>> configure = null)
            where TInstance : class, SagaStateMachineInstance
        {
            ISagaRepository<TInstance> repository = ResolveSagaRepository<TInstance>(kernel);

            var stateMachineConfigurator = new StateMachineSagaConfigurator<TInstance>(stateMachine, repository, configurator);

            configure?.Invoke(stateMachineConfigurator);

            configurator.AddEndpointSpecification(stateMachineConfigurator);
        }

        /// <summary>
        /// Subscribe a state machine saga to the endpoint
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <param name="configurator"></param>
        /// <param name="stateMachine">The state machine</param>
        /// <param name="container">The Windsor Lifetime container to resolve the repository</param>
        /// <param name="configure">Optionally configure the saga</param>
        /// <returns></returns>
        public static void StateMachineSaga<TInstance>(this IReceiveEndpointConfigurator configurator, SagaStateMachine<TInstance> stateMachine,
            IWindsorContainer container, Action<ISagaConfigurator<TInstance>> configure = null)
            where TInstance : class, SagaStateMachineInstance
        {
            RegisterScopedContextProviderIfNotPresent(container);

            StateMachineSaga(configurator, stateMachine, container.Kernel, configure);
        }

        /// <summary>
        /// Subscribe a state machine saga to the endpoint
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <param name="configurator"></param>
        /// <param name="kernel">The Windsor Lifetime Container to resolve the repository</param>
        /// <param name="configure">Optionally configure the saga</param>
        /// <returns></returns>
        public static void StateMachineSaga<TInstance>(this IReceiveEndpointConfigurator configurator, IKernel kernel,
            Action<ISagaConfigurator<TInstance>> configure = null)
            where TInstance : class, SagaStateMachineInstance
        {
            SagaStateMachine<TInstance> stateMachine = kernel.ResolveSagaStateMachine<TInstance>();

            StateMachineSaga(configurator, stateMachine, kernel, configure);
        }

        /// <summary>
        /// Subscribe a state machine saga to the endpoint
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <param name="configurator"></param>
        /// <param name="container">The Windsor Lifetime Container to resolve the repository</param>
        /// <param name="configure">Optionally configure the saga</param>
        /// <returns></returns>
        public static void StateMachineSaga<TInstance>(this IReceiveEndpointConfigurator configurator, IWindsorContainer container,
            Action<ISagaConfigurator<TInstance>> configure = null)
            where TInstance : class, SagaStateMachineInstance
        {
            RegisterScopedContextProviderIfNotPresent(container);

            StateMachineSaga(configurator, container.Kernel, configure);
        }

        public static ConnectHandle ConnectStateMachineSaga<TInstance>(this IConsumePipeConnector pipe, SagaStateMachine<TInstance> stateMachine,
            IKernel kernel)
            where TInstance : class, SagaStateMachineInstance
        {
            var connector = new StateMachineConnector<TInstance>(stateMachine);

            ISagaRepository<TInstance> repository = ResolveSagaRepository<TInstance>(kernel);

            ISagaSpecification<TInstance> specification = connector.CreateSagaSpecification<TInstance>();

            return connector.ConnectSaga(pipe, repository, specification);
        }

        public static ConnectHandle ConnectStateMachineSaga<TInstance>(this IConsumePipeConnector pipe, SagaStateMachine<TInstance> stateMachine,
            IWindsorContainer container)
            where TInstance : class, SagaStateMachineInstance
        {
            RegisterScopedContextProviderIfNotPresent(container);

            return ConnectStateMachineSaga(pipe, stateMachine, container.Kernel);
        }

        public static ConnectHandle ConnectStateMachineSaga<TInstance>(this IConsumePipeConnector pipe, IKernel kernel)
            where TInstance : class, SagaStateMachineInstance
        {
            SagaStateMachine<TInstance> stateMachine = kernel.ResolveSagaStateMachine<TInstance>();

            return pipe.ConnectStateMachineSaga(stateMachine, kernel);
        }

        public static ConnectHandle ConnectStateMachineSaga<TInstance>(this IConsumePipeConnector pipe, IWindsorContainer container)
            where TInstance : class, SagaStateMachineInstance
        {
            return ConnectStateMachineSaga<TInstance>(pipe, container.Kernel);
        }

        static ISagaRepository<TInstance> ResolveSagaRepository<TInstance>(this IKernel kernel)
            where TInstance : class, ISaga
        {
            ISagaRepositoryFactory repositoryFactory = new WindsorSagaRepositoryFactory(kernel);

            return repositoryFactory.CreateSagaRepository<TInstance>();
        }

        static SagaStateMachine<TInstance> ResolveSagaStateMachine<TInstance>(this IKernel kernel)
            where TInstance : class, SagaStateMachineInstance
        {
            return kernel.Resolve<SagaStateMachine<TInstance>>();
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

        internal static void RegisterScopedContextProviderIfNotPresent(this IWindsorContainer container)
        {
            if (!container.Kernel.HasComponent(typeof(ScopedConsumeContextProvider)))
            {
                container.Register(Component.For<ScopedConsumeContextProvider>().LifestyleScoped(),
                    Component.For<ConsumeContext>()
                        .UsingFactoryMethod(kernel => kernel.Resolve<ScopedConsumeContextProvider>().GetContext() ?? new MissingConsumeContext())
                        .LifestyleScoped());
            }
        }
    }
}
