namespace MassTransit
{
    using System;
    using Automatonymous;
    using ConsumeConfigurators;
    using Courier;
    using PipeConfigurators;
    using Saga;
    using Scoping;
    using SimpleInjector;
    using SimpleInjectorIntegration.ScopeProviders;


    public static class SimpleInjectorReceiveEndpointExtensions
    {
        public static void Consumer<T>(this IReceiveEndpointConfigurator configurator, Container container, Action<IConsumerConfigurator<T>> configure = null)
            where T : class, IConsumer
        {
            var consumerFactory = new ScopeConsumerFactory<T>(new SimpleInjectorConsumerScopeProvider(container));

            configurator.Consumer(consumerFactory, configure);
        }

        public static void Consumer<TConsumer, TMessage>(this IBatchConfigurator<TMessage> configurator, Container container,
            Action<IConsumerMessageConfigurator<TConsumer, Batch<TMessage>>> configure = null)
            where TConsumer : class, IConsumer<Batch<TMessage>>
            where TMessage : class
        {
            IConsumerScopeProvider scopeProvider = new SimpleInjectorConsumerScopeProvider(container);

            IConsumerFactory<TConsumer> consumerFactory = new ScopeConsumerFactory<TConsumer>(scopeProvider);

            configurator.Consumer(consumerFactory, configure);
        }

        public static void Saga<T>(this IReceiveEndpointConfigurator configurator, Container container, Action<ISagaConfigurator<T>> configure = null)
            where T : class, ISaga
        {
            configurator.Saga(container.GetInstance<ISagaRepository<T>>(), configure);
        }

        /// <summary>
        /// Subscribe a state machine saga to the endpoint
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <param name="configurator"></param>
        /// <param name="stateMachine"></param>
        /// <param name="container">The SimpleInjector Container to resolve the repository</param>
        /// <param name="configure">Optionally configure the saga</param>
        /// <returns></returns>
        public static void StateMachineSaga<TInstance>(this IReceiveEndpointConfigurator configurator, SagaStateMachine<TInstance> stateMachine,
            Container container, Action<ISagaConfigurator<TInstance>> configure = null)
            where TInstance : class, SagaStateMachineInstance
        {
            configurator.StateMachineSaga(stateMachine, container.GetInstance<ISagaRepository<TInstance>>(), configure);
        }

        /// <summary>
        /// Subscribe a state machine saga to the endpoint
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <param name="configurator"></param>
        /// <param name="container">The SimpleInjector Container to resolve the repository</param>
        /// <param name="configure">Optionally configure the saga</param>
        /// <returns></returns>
        public static void StateMachineSaga<TInstance>(this IReceiveEndpointConfigurator configurator, Container container,
            Action<ISagaConfigurator<TInstance>> configure = null)
            where TInstance : class, SagaStateMachineInstance
        {
            var stateMachine = container.GetInstance<SagaStateMachine<TInstance>>();

            configurator.StateMachineSaga(stateMachine, container.GetInstance<ISagaRepository<TInstance>>(), configure);
        }

        public static void ExecuteActivityHost<TActivity, TArguments>(this IReceiveEndpointConfigurator configurator, Uri compensateAddress,
            Container container,
            Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure = null)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            var executeActivityScopeProvider = new SimpleInjectorExecuteActivityScopeProvider<TActivity, TArguments>(container);

            var factory = new ScopeExecuteActivityFactory<TActivity, TArguments>(executeActivityScopeProvider);

            var specification = new ExecuteActivityHostSpecification<TActivity, TArguments>(factory, compensateAddress, configurator);

            configure?.Invoke(specification);

            configurator.AddEndpointSpecification(specification);
        }

        public static void ExecuteActivityHost<TActivity, TArguments>(this IReceiveEndpointConfigurator configurator, Container container,
            Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure = null)
            where TActivity : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            var executeActivityScopeProvider = new SimpleInjectorExecuteActivityScopeProvider<TActivity, TArguments>(container);

            var factory = new ScopeExecuteActivityFactory<TActivity, TArguments>(executeActivityScopeProvider);

            var specification = new ExecuteActivityHostSpecification<TActivity, TArguments>(factory, configurator);

            configure?.Invoke(specification);

            configurator.AddEndpointSpecification(specification);
        }

        public static void CompensateActivityHost<TActivity, TLog>(this IReceiveEndpointConfigurator configurator, Container container,
            Action<ICompensateActivityConfigurator<TActivity, TLog>> configure = null)
            where TActivity : class, ICompensateActivity<TLog>
            where TLog : class
        {
            var compensateActivityScopeProvider = new SimpleInjectorCompensateActivityScopeProvider<TActivity, TLog>(container);

            var factory = new ScopeCompensateActivityFactory<TActivity, TLog>(compensateActivityScopeProvider);

            var specification = new CompensateActivityHostSpecification<TActivity, TLog>(factory, configurator);

            configure?.Invoke(specification);

            configurator.AddEndpointSpecification(specification);
        }
    }
}
