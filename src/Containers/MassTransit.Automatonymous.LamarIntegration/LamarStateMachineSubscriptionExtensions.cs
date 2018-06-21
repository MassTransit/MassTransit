namespace MassTransit
{
    using System;
    using Automatonymous;
    using Automatonymous.SagaConfigurators;
    using Automatonymous.Scoping;
    using Automatonymous.StateMachineConnectors;
    using AutomatonymousLamarIntegration;
    using GreenPipes;
    using Lamar;
    using Pipeline;
    using Saga;
    using Scoping;

    public static class LamarStateMachineSubscriptionExtensions
    {
        /// <summary>
        /// Subscribe a state machine saga to the endpoint
        /// </summary>
        /// <typeparam name="TInstance">The state machine instance type</typeparam>
        /// <param name="configurator"></param>
        /// <param name="stateMachine">The state machine</param>
        /// <param name="container">The StructureMap Container to resolve the repository</param>
        /// <param name="configure">Optionally configure the saga</param>
        /// <returns></returns>
        public static void StateMachineSaga<TInstance>(this IReceiveEndpointConfigurator configurator, SagaStateMachine<TInstance> stateMachine,
            IContainer container, Action<ISagaConfigurator<TInstance>> configure = null)
            where TInstance : class, SagaStateMachineInstance
        {
            ISagaRepositoryFactory repositoryFactory = new LamarStateMachineSagaRepositoryFactory(container);

            ISagaRepository<TInstance> sagaRepository = repositoryFactory.CreateSagaRepository<TInstance>();

            var stateMachineConfigurator = new StateMachineSagaConfigurator<TInstance>(stateMachine, sagaRepository, configurator);

            configure?.Invoke(stateMachineConfigurator);

            configurator.AddEndpointSpecification(stateMachineConfigurator);
        }

        public static void StateMachineSaga<TInstance>(this IReceiveEndpointConfigurator configurator, IContainer container, Action<ISagaConfigurator<TInstance>> configure = null)
            where TInstance : class, SagaStateMachineInstance
        {
            ISagaStateMachineFactory stateMachineFactory = new LamarSagaStateMachineFactory(container);

            SagaStateMachine<TInstance> stateMachine = stateMachineFactory.CreateStateMachine<TInstance>();

            ISagaRepositoryFactory repositoryFactory = new LamarStateMachineSagaRepositoryFactory(container);

            ISagaRepository<TInstance> sagaRepository = repositoryFactory.CreateSagaRepository<TInstance>();

            var stateMachineConfigurator = new StateMachineSagaConfigurator<TInstance>(stateMachine, sagaRepository, configurator);

            configure?.Invoke(stateMachineConfigurator);

            configurator.AddEndpointSpecification(stateMachineConfigurator);
        }

        public static ConnectHandle ConnectStateMachineSaga<TInstance>(this IConsumePipeConnector bus, SagaStateMachine<TInstance> stateMachine,
            IContainer container)
            where TInstance : class, SagaStateMachineInstance
        {
            var connector = new StateMachineConnector<TInstance>(stateMachine);

            ISagaRepositoryFactory repositoryFactory = new LamarStateMachineSagaRepositoryFactory(container);

            ISagaRepository<TInstance> sagaRepository = repositoryFactory.CreateSagaRepository<TInstance>();

            ISagaSpecification<TInstance> specification = connector.CreateSagaSpecification<TInstance>();

            return connector.ConnectSaga(bus, sagaRepository, specification);
        }
    }
}
