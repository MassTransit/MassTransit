namespace MassTransit
{
    using System;
    using Autofac;
    using AutofacIntegration;
    using AutofacIntegration.Registration;
    using AutofacIntegration.ScopeProviders;
    using Automatonymous;
    using Automatonymous.SagaConfigurators;
    using Automatonymous.StateMachineConnectors;
    using ConsumeConfigurators;
    using Courier;
    using GreenPipes;
    using PipeConfigurators;
    using Pipeline;
    using Registration;
    using Saga;
    using Scoping;


    public static class AutofacReceiveEndpointExtensions
    {
        /// <summary>
        /// Registers a consumer given the lifetime scope specified
        /// </summary>
        /// <typeparam name="T">The consumer type</typeparam>
        /// <param name="configurator">The service bus configurator</param>
        /// <param name="context">The LifetimeScope of the container</param>
        /// <param name="name">The name of the scope created per-message</param>
        /// <param name="configureScope">Configuration for scope container</param>
        /// <returns></returns>
        public static void Consumer<T>(this IReceiveEndpointConfigurator configurator, IComponentContext context, string name = "message",
            Action<ContainerBuilder, ConsumeContext> configureScope = null)
            where T : class, IConsumer
        {
            Consumer<T>(configurator, context.Resolve<ILifetimeScope>(), name, configureScope);
        }

        /// <summary>
        /// Registers a consumer given the lifetime scope specified
        /// </summary>
        /// <typeparam name="T">The consumer type</typeparam>
        /// <param name="configurator">The service bus configurator</param>
        /// <param name="scope">The LifetimeScope of the container</param>
        /// <param name="name">The name of the scope created per-message</param>
        /// <param name="configureScope">Configuration for scope container</param>
        /// <returns></returns>
        public static void Consumer<T>(this IReceiveEndpointConfigurator configurator, ILifetimeScope scope, string name = "message",
            Action<ContainerBuilder, ConsumeContext> configureScope = null)
            where T : class, IConsumer
        {
            IConsumerScopeProvider scopeProvider = new AutofacConsumerScopeProvider(new SingleLifetimeScopeProvider(scope), name, configureScope);

            var consumerFactory = new ScopeConsumerFactory<T>(scopeProvider);

            configurator.Consumer(consumerFactory);
        }

        /// <summary>
        /// Registers a consumer given the lifetime scope specified
        /// </summary>
        /// <typeparam name="T">The consumer type</typeparam>
        /// <typeparam name="TId"></typeparam>
        /// <param name="configurator">The service bus configurator</param>
        /// <param name="context">The component context containing the registry</param>
        /// <param name="name">The name of the scope created per-message</param>
        /// <param name="configureScope">Configuration for scope container</param>
        /// <returns></returns>
        public static void ConsumerInScope<T, TId>(this IReceiveEndpointConfigurator configurator, IComponentContext context, string name = "message",
            Action<ContainerBuilder, ConsumeContext> configureScope = null)
            where T : class, IConsumer
        {
            var registry = context.Resolve<ILifetimeScopeRegistry<TId>>();

            IConsumerScopeProvider scopeProvider = new AutofacConsumerScopeProvider(new RegistryLifetimeScopeProvider<TId>(registry), name, configureScope);

            var consumerFactory = new ScopeConsumerFactory<T>(scopeProvider);

            configurator.Consumer(consumerFactory);
        }

        /// <summary>
        /// Registers a consumer given the lifetime scope specified
        /// </summary>
        /// <typeparam name="T">The consumer type</typeparam>
        /// <param name="configurator">The service bus configurator</param>
        /// <param name="context">The LifetimeScope of the container</param>
        /// <param name="configure"></param>
        /// <param name="name">The name of the scope created per-message</param>
        /// <param name="configureScope">Configuration for scope container</param>
        /// <returns></returns>
        public static void Consumer<T>(this IReceiveEndpointConfigurator configurator, IComponentContext context, Action<IConsumerConfigurator<T>> configure,
            string name = "message",
            Action<ContainerBuilder, ConsumeContext> configureScope = null)
            where T : class, IConsumer
        {
            Consumer(configurator, context.Resolve<ILifetimeScope>(), configure, name, configureScope);
        }

        /// <summary>
        /// Registers a consumer given the lifetime scope specified
        /// </summary>
        /// <typeparam name="T">The consumer type</typeparam>
        /// <param name="configurator">The service bus configurator</param>
        /// <param name="scope">The LifetimeScope of the container</param>
        /// <param name="configure"></param>
        /// <param name="name">The name of the scope created per-message</param>
        /// <param name="configureScope">Configuration for scope container</param>
        /// <returns></returns>
        public static void Consumer<T>(this IReceiveEndpointConfigurator configurator, ILifetimeScope scope, Action<IConsumerConfigurator<T>> configure,
            string name = "message",
            Action<ContainerBuilder, ConsumeContext> configureScope = null)
            where T : class, IConsumer
        {
            IConsumerScopeProvider scopeProvider = new AutofacConsumerScopeProvider(new SingleLifetimeScopeProvider(scope), name, configureScope);

            var consumerFactory = new ScopeConsumerFactory<T>(scopeProvider);

            configurator.Consumer(consumerFactory, configure);
        }

        /// <summary>
        /// Connect a consumer with a consumer factory method
        /// </summary>
        /// <typeparam name="TConsumer"></typeparam>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="scope"></param>
        /// <param name="configure"></param>
        /// <param name="name">The name of the scope created per-message</param>
        /// <param name="configureScope">Configuration for scope container</param>
        /// <returns></returns>
        public static void Consumer<TConsumer, TMessage>(this IBatchConfigurator<TMessage> configurator, ILifetimeScope scope,
            Action<IConsumerMessageConfigurator<TConsumer, Batch<TMessage>>> configure = null, string name = "message",
            Action<ContainerBuilder, ConsumeContext> configureScope = null)
            where TConsumer : class, IConsumer<Batch<TMessage>>
            where TMessage : class
        {
            IConsumerScopeProvider scopeProvider = new AutofacConsumerScopeProvider(new SingleLifetimeScopeProvider(scope), name, configureScope);

            IConsumerFactory<TConsumer> consumerFactory = new ScopeConsumerFactory<TConsumer>(scopeProvider);

            configurator.Consumer(consumerFactory, configure);
        }

        /// <summary>
        /// Connect a consumer with a consumer factory method
        /// </summary>
        /// <typeparam name="TConsumer"></typeparam>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="context"></param>
        /// <param name="configure"></param>
        /// <param name="name">The name of the scope created per-message</param>
        /// <param name="configureScope">Configuration for scope container</param>
        /// <returns></returns>
        public static void Consumer<TConsumer, TMessage>(this IBatchConfigurator<TMessage> configurator, IComponentContext context,
            Action<IConsumerMessageConfigurator<TConsumer, Batch<TMessage>>> configure = null, string name = "message",
            Action<ContainerBuilder, ConsumeContext> configureScope = null)
            where TConsumer : class, IConsumer<Batch<TMessage>>
            where TMessage : class
        {
            Consumer(configurator, context.Resolve<ILifetimeScope>(), configure, name, configureScope);
        }

        /// <summary>
        /// Registers a saga using the container that has the repository resolved from the container
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="context"></param>
        /// <param name="name"></param>
        /// <param name="configureScope">Configuration for scope container</param>
        /// <returns></returns>
        public static void Saga<T>(this IReceiveEndpointConfigurator configurator, IComponentContext context, string name = "message",
            Action<ContainerBuilder, ConsumeContext> configureScope = null)
            where T : class, ISaga
        {
            Saga<T>(configurator, context.Resolve<ILifetimeScope>(), name, configureScope);
        }

        /// <summary>
        /// Registers a saga using the container that has the repository resolved from the container
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="scope"></param>
        /// <param name="name"></param>
        /// <param name="configureScope">Configuration for scope container</param>
        /// <returns></returns>
        public static void Saga<T>(this IReceiveEndpointConfigurator configurator, ILifetimeScope scope, string name = "message",
            Action<ContainerBuilder, ConsumeContext> configureScope = null)
            where T : class, ISaga
        {
            ISagaRepository<T> repository = ResolveSagaRepository<T>(scope, name, configureScope);

            configurator.Saga(repository);
        }

        /// <summary>
        /// Registers a saga using the container that has the repository resolved from the container
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="context"></param>
        /// <param name="configure"></param>
        /// <param name="name"></param>
        /// <param name="configureScope">Configuration for scope container</param>
        /// <returns></returns>
        public static void Saga<T>(this IReceiveEndpointConfigurator configurator, IComponentContext context, Action<ISagaConfigurator<T>> configure,
            string name = "message", Action<ContainerBuilder, ConsumeContext> configureScope = null)
            where T : class, ISaga
        {
            Saga(configurator, context.Resolve<ILifetimeScope>(), configure, name, configureScope);
        }

        /// <summary>
        /// Registers a saga using the container that has the repository resolved from the container
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="scope"></param>
        /// <param name="configure"></param>
        /// <param name="name"></param>
        /// <param name="configureScope">Configuration for scope container</param>
        /// <returns></returns>
        public static void Saga<T>(this IReceiveEndpointConfigurator configurator, ILifetimeScope scope, Action<ISagaConfigurator<T>> configure,
            string name = "message", Action<ContainerBuilder, ConsumeContext> configureScope = null)
            where T : class, ISaga
        {
            ISagaRepository<T> repository = ResolveSagaRepository<T>(scope, name, configureScope);

            configurator.Saga(repository, configure);
        }

        /// <summary>
        /// Subscribe a state machine saga to the endpoint
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <param name="configurator"></param>
        /// <param name="stateMachine">The state machine</param>
        /// <param name="context">The Autofac root container to resolve the repository</param>
        /// <param name="configure">Optionally configure the saga</param>
        /// <param name="name">The name to use for the scope created for each message</param>
        /// <param name="configureScope">Configuration for scope container</param>
        /// <returns></returns>
        public static void StateMachineSaga<TInstance>(this IReceiveEndpointConfigurator configurator, SagaStateMachine<TInstance> stateMachine,
            IComponentContext context, Action<ISagaConfigurator<TInstance>> configure = null, string name = "message",
            Action<ContainerBuilder, ConsumeContext> configureScope = null)
            where TInstance : class, SagaStateMachineInstance
        {
            StateMachineSaga(configurator, stateMachine, context.Resolve<ILifetimeScope>(), configure, name, configureScope);
        }

        /// <summary>
        /// Subscribe a state machine saga to the endpoint
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <param name="configurator"></param>
        /// <param name="stateMachine">The state machine</param>
        /// <param name="scope">The Autofac Lifetime container to resolve the repository</param>
        /// <param name="configure">Optionally configure the saga</param>
        /// <param name="name">The name to use for the scope created for each message</param>
        /// <param name="configureScope">Configuration for scope container</param>
        /// <returns></returns>
        public static void StateMachineSaga<TInstance>(this IReceiveEndpointConfigurator configurator, SagaStateMachine<TInstance> stateMachine,
            ILifetimeScope scope, Action<ISagaConfigurator<TInstance>> configure = null, string name = "message",
            Action<ContainerBuilder, ConsumeContext> configureScope = null)
            where TInstance : class, SagaStateMachineInstance
        {
            ISagaRepository<TInstance> repository = ResolveSagaRepository<TInstance>(scope, name, configureScope);

            var stateMachineConfigurator = new StateMachineSagaConfigurator<TInstance>(stateMachine, repository, configurator);

            configure?.Invoke(stateMachineConfigurator);

            configurator.AddEndpointSpecification(stateMachineConfigurator);
        }

        /// <summary>
        /// Subscribe a state machine saga to the endpoint
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <param name="configurator"></param>
        /// <param name="context">The Autofac root container to resolve the repository</param>
        /// <param name="configure">Optionally configure the saga</param>
        /// <param name="name">The name to use for the scope created for each message</param>
        /// <param name="configureScope">Configuration for scope container</param>
        /// <returns></returns>
        public static void StateMachineSaga<TInstance>(this IReceiveEndpointConfigurator configurator, IComponentContext context,
            Action<ISagaConfigurator<TInstance>> configure = null, string name = "message", Action<ContainerBuilder, ConsumeContext> configureScope = null)
            where TInstance : class, SagaStateMachineInstance
        {
            StateMachineSaga(configurator, context.Resolve<ILifetimeScope>(), configure, name, configureScope);
        }

        /// <summary>
        /// Subscribe a state machine saga to the endpoint
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <param name="configurator"></param>
        /// <param name="scope">The Autofac Lifetime Container to resolve the repository</param>
        /// <param name="configure">Optionally configure the saga</param>
        /// <param name="name">The name to use for the scope created for each message</param>
        /// <param name="configureScope">Configuration for scope container</param>
        /// <returns></returns>
        public static void StateMachineSaga<TInstance>(this IReceiveEndpointConfigurator configurator, ILifetimeScope scope,
            Action<ISagaConfigurator<TInstance>> configure = null, string name = "message", Action<ContainerBuilder, ConsumeContext> configureScope = null)
            where TInstance : class, SagaStateMachineInstance
        {
            SagaStateMachine<TInstance> stateMachine = ResolveSagaStateMachine<TInstance>(scope);

            StateMachineSaga(configurator, stateMachine, scope, configure, name, configureScope);
        }

        public static ConnectHandle ConnectStateMachineSaga<TInstance>(this IConsumePipeConnector pipe, SagaStateMachine<TInstance> stateMachine,
            ILifetimeScope scope, string name = "message", Action<ContainerBuilder, ConsumeContext> configureScope = null)
            where TInstance : class, SagaStateMachineInstance
        {
            var connector = new StateMachineConnector<TInstance>(stateMachine);

            ISagaRepository<TInstance> repository = ResolveSagaRepository<TInstance>(scope, name, configureScope);

            ISagaSpecification<TInstance> specification = connector.CreateSagaSpecification<TInstance>();

            return connector.ConnectSaga(pipe, repository, specification);
        }

        public static ConnectHandle ConnectStateMachineSaga<TInstance>(this IConsumePipeConnector pipe, ILifetimeScope scope, string name = "message",
            Action<ContainerBuilder, ConsumeContext> configureScope = null)
            where TInstance : class, SagaStateMachineInstance
        {
            SagaStateMachine<TInstance> stateMachine = ResolveSagaStateMachine<TInstance>(scope);

            return pipe.ConnectStateMachineSaga(stateMachine, scope, name, configureScope);
        }

        static ISagaRepository<TInstance> ResolveSagaRepository<TInstance>(this ILifetimeScope scope, string name = "message",
            Action<ContainerBuilder, ConsumeContext> configureScope = null)
            where TInstance : class, ISaga
        {
            ISagaRepositoryFactory repositoryFactory = new AutofacSagaRepositoryFactory(new SingleLifetimeScopeProvider(scope), name, configureScope);

            return repositoryFactory.CreateSagaRepository<TInstance>();
        }

        static SagaStateMachine<TInstance> ResolveSagaStateMachine<TInstance>(this ILifetimeScope scope)
            where TInstance : class, SagaStateMachineInstance
        {
            return scope.Resolve<SagaStateMachine<TInstance>>();
        }

        public static void ExecuteActivityHost<TActivity, TArguments>(this IReceiveEndpointConfigurator configurator, Uri compensateAddress,
            IComponentContext context,
            string name = "message",
            Action<ContainerBuilder, ExecuteContext<TArguments>> configureScope = null)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            ExecuteActivityHost<TActivity, TArguments>(configurator, compensateAddress, context.Resolve<ILifetimeScope>(), name, configureScope);
        }

        public static void ExecuteActivityHost<TActivity, TArguments>(this IReceiveEndpointConfigurator configurator, Uri compensateAddress,
            IComponentContext context,
            Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure,
            string name = "message",
            Action<ContainerBuilder, ExecuteContext<TArguments>> configureScope = null)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            ExecuteActivityHost(configurator, compensateAddress, context.Resolve<ILifetimeScope>(), configure, name, configureScope);
        }

        public static void ExecuteActivityHost<TActivity, TArguments>(this IReceiveEndpointConfigurator configurator, Uri compensateAddress,
            ILifetimeScope lifetimeScope,
            string name = "message",
            Action<ContainerBuilder, ExecuteContext<TArguments>> configureScope = null)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            var lifetimeScopeProvider = new SingleLifetimeScopeProvider(lifetimeScope);

            var executeActivityScopeProvider = new AutofacExecuteActivityScopeProvider<TActivity, TArguments>(lifetimeScopeProvider, name, configureScope);

            var factory = new ScopeExecuteActivityFactory<TActivity, TArguments>(executeActivityScopeProvider);

            var specification = new ExecuteActivityHostSpecification<TActivity, TArguments>(factory, compensateAddress, configurator);

            configurator.AddEndpointSpecification(specification);
        }

        public static void ExecuteActivityHost<TActivity, TArguments>(this IReceiveEndpointConfigurator configurator, Uri compensateAddress,
            ILifetimeScope lifetimeScope,
            Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure,
            string name = "message",
            Action<ContainerBuilder, ExecuteContext<TArguments>> configureScope = null)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            var lifetimeScopeProvider = new SingleLifetimeScopeProvider(lifetimeScope);

            var executeActivityScopeProvider = new AutofacExecuteActivityScopeProvider<TActivity, TArguments>(lifetimeScopeProvider, name, configureScope);

            var factory = new ScopeExecuteActivityFactory<TActivity, TArguments>(executeActivityScopeProvider);

            var specification = new ExecuteActivityHostSpecification<TActivity, TArguments>(factory, compensateAddress, configurator);

            configure(specification);

            configurator.AddEndpointSpecification(specification);
        }

        public static void ExecuteActivityHost<TActivity, TArguments>(this IReceiveEndpointConfigurator configurator,
            IComponentContext context,
            string name = "message",
            Action<ContainerBuilder, ExecuteContext<TArguments>> configureScope = null)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            ExecuteActivityHost<TActivity, TArguments>(configurator, context.Resolve<ILifetimeScope>(), name, configureScope);
        }

        public static void ExecuteActivityHost<TActivity, TArguments>(this IReceiveEndpointConfigurator configurator,
            IComponentContext context,
            Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure,
            string name = "message",
            Action<ContainerBuilder, ExecuteContext<TArguments>> configureScope = null)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            ExecuteActivityHost(configurator, context.Resolve<ILifetimeScope>(), configure, name, configureScope);
        }

        public static void ExecuteActivityHost<TActivity, TArguments>(this IReceiveEndpointConfigurator configurator, ILifetimeScope lifetimeScope,
            string name = "message",
            Action<ContainerBuilder, ExecuteContext<TArguments>> configureScope = null)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            var lifetimeScopeProvider = new SingleLifetimeScopeProvider(lifetimeScope);

            var executeActivityScopeProvider = new AutofacExecuteActivityScopeProvider<TActivity, TArguments>(lifetimeScopeProvider, name, configureScope);

            var factory = new ScopeExecuteActivityFactory<TActivity, TArguments>(executeActivityScopeProvider);

            var specification = new ExecuteActivityHostSpecification<TActivity, TArguments>(factory, configurator);

            configurator.AddEndpointSpecification(specification);
        }

        public static void ExecuteActivityHost<TActivity, TArguments>(this IReceiveEndpointConfigurator configurator, ILifetimeScope lifetimeScope,
            Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure,
            string name = "message",
            Action<ContainerBuilder, ExecuteContext<TArguments>> configureScope = null)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            var lifetimeScopeProvider = new SingleLifetimeScopeProvider(lifetimeScope);

            var executeActivityScopeProvider = new AutofacExecuteActivityScopeProvider<TActivity, TArguments>(lifetimeScopeProvider, name, configureScope);

            var factory = new ScopeExecuteActivityFactory<TActivity, TArguments>(executeActivityScopeProvider);

            var specification = new ExecuteActivityHostSpecification<TActivity, TArguments>(factory, configurator);

            configure(specification);

            configurator.AddEndpointSpecification(specification);
        }

        public static void CompensateActivityHost<TActivity, TLog>(this IReceiveEndpointConfigurator configurator, IComponentContext context,
            string name = "message",
            Action<ContainerBuilder, CompensateContext<TLog>> configureScope = null)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            CompensateActivityHost<TActivity, TLog>(configurator, context.Resolve<ILifetimeScope>(), name, configureScope);
        }

        public static void CompensateActivityHost<TActivity, TLog>(this IReceiveEndpointConfigurator configurator, IComponentContext context,
            Action<ICompensateActivityConfigurator<TActivity, TLog>> configure,
            string name = "message",
            Action<ContainerBuilder, CompensateContext<TLog>> configureScope = null)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            CompensateActivityHost(configurator, context.Resolve<ILifetimeScope>(), configure, name, configureScope);
        }

        public static void CompensateActivityHost<TActivity, TLog>(this IReceiveEndpointConfigurator configurator, ILifetimeScope lifetimeScope,
            string name = "message",
            Action<ContainerBuilder, CompensateContext<TLog>> configureScope = null)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            var lifetimeScopeProvider = new SingleLifetimeScopeProvider(lifetimeScope);

            var compensateActivityScopeProvider = new AutofacCompensateActivityScopeProvider<TActivity, TLog>(lifetimeScopeProvider, name, configureScope);

            var factory = new ScopeCompensateActivityFactory<TActivity, TLog>(compensateActivityScopeProvider);

            var specification = new CompensateActivityHostSpecification<TActivity, TLog>(factory, configurator);

            configurator.AddEndpointSpecification(specification);
        }

        public static void CompensateActivityHost<TActivity, TLog>(this IReceiveEndpointConfigurator configurator, ILifetimeScope lifetimeScope,
            Action<ICompensateActivityConfigurator<TActivity, TLog>> configure,
            string name = "message",
            Action<ContainerBuilder, CompensateContext<TLog>> configureScope = null)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            var lifetimeScopeProvider = new SingleLifetimeScopeProvider(lifetimeScope);

            var compensateActivityScopeProvider = new AutofacCompensateActivityScopeProvider<TActivity, TLog>(lifetimeScopeProvider, name, configureScope);

            var factory = new ScopeCompensateActivityFactory<TActivity, TLog>(compensateActivityScopeProvider);

            var specification = new CompensateActivityHostSpecification<TActivity, TLog>(factory, configurator);

            configure(specification);

            configurator.AddEndpointSpecification(specification);
        }
    }
}
