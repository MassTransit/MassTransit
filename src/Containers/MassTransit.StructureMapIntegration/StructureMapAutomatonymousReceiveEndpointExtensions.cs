namespace MassTransit
{
    using System;
    using Automatonymous;
    using Automatonymous.SagaConfigurators;
    using Automatonymous.StateMachineConnectors;
    using GreenPipes;
    using Pipeline;
    using Registration;
    using Saga;
    using StructureMap;
    using StructureMapIntegration;
    using StructureMapIntegration.Registration;


    public static class StructureMapAutomatonymousReceiveEndpointExtensions
    {
        static readonly IStateMachineActivityFactory _activityFactory = new StructureMapStateMachineActivityFactory();

        /// <summary>
        /// Subscribe a state machine saga to the endpoint
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <param name="configurator"></param>
        /// <param name="stateMachine">The state machine</param>
        /// <param name="context">The StructureMap root container to resolve the repository</param>
        /// <param name="configure">Optionally configure the saga</param>
        /// <returns></returns>
        public static void StateMachineSaga<TInstance>(this IReceiveEndpointConfigurator configurator, SagaStateMachine<TInstance> stateMachine,
            IContext context, Action<ISagaConfigurator<TInstance>> configure = null)
            where TInstance : class, SagaStateMachineInstance
        {
            StateMachineSaga(configurator, stateMachine, context.GetInstance<IContainer>(), configure);
        }

        /// <summary>
        /// Subscribe a state machine saga to the endpoint
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <param name="configurator"></param>
        /// <param name="stateMachine">The state machine</param>
        /// <param name="container">The StructureMap Lifetime container to resolve the repository</param>
        /// <param name="configure">Optionally configure the saga</param>
        /// <returns></returns>
        public static void StateMachineSaga<TInstance>(this IReceiveEndpointConfigurator configurator, SagaStateMachine<TInstance> stateMachine,
            IContainer container, Action<ISagaConfigurator<TInstance>> configure = null)
            where TInstance : class, SagaStateMachineInstance
        {
            ISagaRepository<TInstance> repository = CreateSagaRepository<TInstance>(container);

            var stateMachineConfigurator = new StateMachineSagaConfigurator<TInstance>(stateMachine, repository, configurator);

            configure?.Invoke(stateMachineConfigurator);

            configurator.AddEndpointSpecification(stateMachineConfigurator);
        }

        /// <summary>
        /// Subscribe a state machine saga to the endpoint
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <param name="configurator"></param>
        /// <param name="context">The StructureMap root container to resolve the repository</param>
        /// <param name="configure">Optionally configure the saga</param>
        /// <returns></returns>
        public static void StateMachineSaga<TInstance>(this IReceiveEndpointConfigurator configurator, IContext context,
            Action<ISagaConfigurator<TInstance>> configure = null)
            where TInstance : class, SagaStateMachineInstance
        {
            StateMachineSaga(configurator, context.GetInstance<IContainer>(), configure);
        }

        /// <summary>
        /// Subscribe a state machine saga to the endpoint
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <param name="configurator"></param>
        /// <param name="container">The StructureMap Lifetime Container to resolve the repository</param>
        /// <param name="configure">Optionally configure the saga</param>
        /// <returns></returns>
        public static void StateMachineSaga<TInstance>(this IReceiveEndpointConfigurator configurator, IContainer container,
            Action<ISagaConfigurator<TInstance>> configure = null)
            where TInstance : class, SagaStateMachineInstance
        {
            ISagaStateMachineFactory stateMachineFactory = new StructureMapSagaStateMachineFactory(container);

            SagaStateMachine<TInstance> stateMachine = stateMachineFactory.CreateStateMachine<TInstance>();

            StateMachineSaga(configurator, stateMachine, container, configure);
        }

        public static ConnectHandle ConnectStateMachineSaga<TInstance>(this IConsumePipeConnector pipe, SagaStateMachine<TInstance> stateMachine,
            IContainer container)
            where TInstance : class, SagaStateMachineInstance
        {
            var connector = new StateMachineConnector<TInstance>(stateMachine);

            ISagaRepository<TInstance> repository = CreateSagaRepository<TInstance>(container);

            ISagaSpecification<TInstance> specification = connector.CreateSagaSpecification<TInstance>();

            return connector.ConnectSaga(pipe, repository, specification);
        }

        public static ConnectHandle ConnectStateMachineSaga<TInstance>(this IConsumePipeConnector pipe, IContainer container)
            where TInstance : class, SagaStateMachineInstance
        {
            ISagaStateMachineFactory stateMachineFactory = new StructureMapSagaStateMachineFactory(container);

            SagaStateMachine<TInstance> stateMachine = stateMachineFactory.CreateStateMachine<TInstance>();

            return pipe.ConnectStateMachineSaga(stateMachine, container);
        }

        static ISagaRepository<TInstance> CreateSagaRepository<TInstance>(IContainer container)
            where TInstance : class, SagaStateMachineInstance
        {
            ISagaRepositoryFactory repositoryFactory = new StructureMapSagaRepositoryFactory(container);

            return repositoryFactory.CreateSagaRepository<TInstance>(AddStateMachineActivityFactory);
        }

        static void AddStateMachineActivityFactory(ConsumeContext context)
        {
            context.GetOrAddPayload(() => _activityFactory);
        }
    }
}
