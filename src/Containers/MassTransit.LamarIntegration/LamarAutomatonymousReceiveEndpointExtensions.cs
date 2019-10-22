namespace MassTransit
{
    using System;
    using Automatonymous;
    using Automatonymous.SagaConfigurators;
    using Automatonymous.StateMachineConnectors;
    using GreenPipes;
    using Lamar;
    using LamarIntegration;
    using LamarIntegration.Registration;
    using Pipeline;
    using Registration;
    using Saga;


    public static class LamarAutomatonymousReceiveEndpointExtensions
    {
        static readonly IStateMachineActivityFactory _activityFactory = new LamarStateMachineActivityFactory();

        /// <summary>
        /// Subscribe a state machine saga to the endpoint
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <param name="configurator"></param>
        /// <param name="stateMachine">The state machine</param>
        /// <param name="context">The Lamar root container to resolve the repository</param>
        /// <param name="configure">Optionally configure the saga</param>
        /// <param name="configureScope">Configuration for scope container</param>
        /// <returns></returns>
        public static void StateMachineSaga<TInstance>(this IReceiveEndpointConfigurator configurator, SagaStateMachine<TInstance> stateMachine,
            IServiceContext context, Action<ISagaConfigurator<TInstance>> configure = null, Action<ConsumeContext> configureScope = null)
            where TInstance : class, SagaStateMachineInstance
        {
            StateMachineSaga(configurator, stateMachine, context.GetInstance<IContainer>(), configure, configureScope);
        }

        /// <summary>
        /// Subscribe a state machine saga to the endpoint
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <param name="configurator"></param>
        /// <param name="stateMachine">The state machine</param>
        /// <param name="scope">The Lamar Lifetime container to resolve the repository</param>
        /// <param name="configure">Optionally configure the saga</param>
        /// <param name="configureScope">Configuration for scope container</param>
        /// <returns></returns>
        public static void StateMachineSaga<TInstance>(this IReceiveEndpointConfigurator configurator, SagaStateMachine<TInstance> stateMachine,
            IContainer scope, Action<ISagaConfigurator<TInstance>> configure = null, Action<ConsumeContext> configureScope = null)
            where TInstance : class, SagaStateMachineInstance
        {
            ISagaRepository<TInstance> repository = CreateSagaRepository<TInstance>(scope, configureScope);

            var stateMachineConfigurator = new StateMachineSagaConfigurator<TInstance>(stateMachine, repository, configurator);

            configure?.Invoke(stateMachineConfigurator);

            configurator.AddEndpointSpecification(stateMachineConfigurator);
        }

        /// <summary>
        /// Subscribe a state machine saga to the endpoint
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <param name="configurator"></param>
        /// <param name="context">The Lamar root container to resolve the repository</param>
        /// <param name="configure">Optionally configure the saga</param>
        /// <param name="configureScope">Configuration for scope container</param>
        /// <returns></returns>
        public static void StateMachineSaga<TInstance>(this IReceiveEndpointConfigurator configurator, IServiceContext context,
            Action<ISagaConfigurator<TInstance>> configure = null, Action<ConsumeContext> configureScope = null)
            where TInstance : class, SagaStateMachineInstance
        {
            StateMachineSaga(configurator, context.GetInstance<IContainer>(), configure, configureScope);
        }

        /// <summary>
        /// Subscribe a state machine saga to the endpoint
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <param name="configurator"></param>
        /// <param name="container">The Lamar Lifetime Container to resolve the repository</param>
        /// <param name="configure">Optionally configure the saga</param>
        /// <param name="configureScope">Configuration for scope container</param>
        /// <returns></returns>
        public static void StateMachineSaga<TInstance>(this IReceiveEndpointConfigurator configurator, IContainer container,
            Action<ISagaConfigurator<TInstance>> configure = null, Action<ConsumeContext> configureScope = null)
            where TInstance : class, SagaStateMachineInstance
        {
            ISagaStateMachineFactory stateMachineFactory = new LamarSagaStateMachineFactory(container);

            SagaStateMachine<TInstance> stateMachine = stateMachineFactory.CreateStateMachine<TInstance>();

            StateMachineSaga(configurator, stateMachine, container, configure, configureScope);
        }

        public static ConnectHandle ConnectStateMachineSaga<TInstance>(this IConsumePipeConnector pipe, SagaStateMachine<TInstance> stateMachine,
            IContainer scope, Action<ConsumeContext> configureScope = null)
            where TInstance : class, SagaStateMachineInstance
        {
            var connector = new StateMachineConnector<TInstance>(stateMachine);

            ISagaRepository<TInstance> repository = CreateSagaRepository<TInstance>(scope, configureScope);

            ISagaSpecification<TInstance> specification = connector.CreateSagaSpecification<TInstance>();

            return connector.ConnectSaga(pipe, repository, specification);
        }

        public static ConnectHandle ConnectStateMachineSaga<TInstance>(this IConsumePipeConnector pipe, IContainer scope,
            Action<ConsumeContext> configureScope = null)
            where TInstance : class, SagaStateMachineInstance
        {
            ISagaStateMachineFactory stateMachineFactory = new LamarSagaStateMachineFactory(scope);

            SagaStateMachine<TInstance> stateMachine = stateMachineFactory.CreateStateMachine<TInstance>();

            return pipe.ConnectStateMachineSaga(stateMachine, scope, configureScope);
        }

        static ISagaRepository<TInstance> CreateSagaRepository<TInstance>(IContainer scope, Action<ConsumeContext> configureScope)
            where TInstance : class, SagaStateMachineInstance
        {
            ISagaRepositoryFactory repositoryFactory = new LamarSagaRepositoryFactory(scope, configureScope);

            return repositoryFactory.CreateSagaRepository<TInstance>(AddStateMachineActivityFactory);
        }

        static void AddStateMachineActivityFactory(ConsumeContext context)
        {
            context.GetOrAddPayload(() => _activityFactory);
        }
    }
}
