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


    public static class AutomatonymousReceiveEndpointExtensions
    {
        /// <summary>
        /// Subscribe a state machine saga to the endpoint
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <param name="configurator"></param>
        /// <param name="stateMachine">The state machine</param>
        /// <param name="repository">The saga repository for the instances</param>
        /// <param name="configure">Optionally configure the saga</param>
        /// <returns></returns>
        public static void StateMachineSaga<TInstance>(this IReceiveEndpointConfigurator configurator, SagaStateMachine<TInstance> stateMachine,
            ISagaRepository<TInstance> repository, Action<ISagaConfigurator<TInstance>> configure = null)
            where TInstance : class, SagaStateMachineInstance
        {
            var stateMachineConfigurator = new StateMachineSagaConfigurator<TInstance>(stateMachine, repository, configurator);

            configure?.Invoke(stateMachineConfigurator);

            configurator.AddEndpointSpecification(stateMachineConfigurator);
        }

        /// <summary>
        /// Subscribe a state machine saga to the endpoint using factories to resolve the required components
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <param name="configurator"></param>
        /// <param name="stateMachine"></param>
        /// <param name="repositoryFactory"></param>
        /// <param name="configure">Optionally configure the saga</param>
        /// <returns></returns>
        public static void StateMachineSaga<TInstance>(this IReceiveEndpointConfigurator configurator, SagaStateMachine<TInstance> stateMachine,
            ISagaRepositoryFactory repositoryFactory, Action<ISagaConfigurator<TInstance>> configure = null)
            where TInstance : class, SagaStateMachineInstance
        {
            var repository = repositoryFactory.CreateSagaRepository<TInstance>();

            StateMachineSaga(configurator, stateMachine, repository, configure);
        }

        /// <summary>
        /// Subscribe a state machine saga to the endpoint using factories to resolve the required components
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <param name="configurator"></param>
        /// <param name="stateMachineFactory"></param>
        /// <param name="repositoryFactory"></param>
        /// <param name="configure">Optionally configure the saga</param>
        /// <returns></returns>
        public static void StateMachineSaga<TInstance>(this IReceiveEndpointConfigurator configurator, ISagaStateMachineFactory stateMachineFactory,
            ISagaRepositoryFactory repositoryFactory, Action<ISagaConfigurator<TInstance>> configure = null)
            where TInstance : class, SagaStateMachineInstance
        {
            var stateMachine = stateMachineFactory.CreateStateMachine<TInstance>();

            var repository = repositoryFactory.CreateSagaRepository<TInstance>();

            StateMachineSaga(configurator, stateMachine, repository, configure);
        }

        public static ConnectHandle ConnectStateMachineSaga<TInstance>(this IConsumePipeConnector bus, SagaStateMachine<TInstance> stateMachine,
            ISagaRepository<TInstance> repository, Action<ISagaConfigurator<TInstance>> configure = null)
            where TInstance : class, SagaStateMachineInstance
        {
            var connector = new StateMachineConnector<TInstance>(stateMachine);

            ISagaSpecification<TInstance> specification = connector.CreateSagaSpecification<TInstance>();

            configure?.Invoke(specification);

            return connector.ConnectSaga(bus, repository, specification);
        }

        public static ConnectHandle ConnectStateMachineSaga<TInstance>(this IConsumePipeConnector bus, SagaStateMachine<TInstance> stateMachine,
            ISagaRepositoryFactory repositoryFactory, Action<ISagaConfigurator<TInstance>> configure = null)
            where TInstance : class, SagaStateMachineInstance
        {
            var repository = repositoryFactory.CreateSagaRepository<TInstance>();

            return ConnectStateMachineSaga(bus, stateMachine, repository, configure);
        }

        public static ConnectHandle ConnectStateMachineSaga<TInstance>(this IConsumePipeConnector bus, ISagaStateMachineFactory stateMachineFactory,
            ISagaRepositoryFactory repositoryFactory, Action<ISagaConfigurator<TInstance>> configure = null)
            where TInstance : class, SagaStateMachineInstance
        {
            var stateMachine = stateMachineFactory.CreateStateMachine<TInstance>();

            var repository = repositoryFactory.CreateSagaRepository<TInstance>();

            return ConnectStateMachineSaga(bus, stateMachine, repository, configure);
        }
    }
}
