namespace MassTransit
{
    using System;
    using Automatonymous;
    using GreenPipes;
    using Pipeline;
    using Registration;
    using Saga;
    using SimpleInjector;
    using SimpleInjectorIntegration;
    using SimpleInjectorIntegration.Registration;


    public static class SimpleInjectorAutomatonymousReceiveEndpointExtensions
    {
        static readonly IStateMachineActivityFactory _activityFactory = new SimpleInjectorStateMachineActivityFactory();

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
            ISagaRepository<TInstance> repository = CreateSagaRepository<TInstance>(container);

            configurator.StateMachineSaga(stateMachine, repository, configure);
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

            ISagaRepository<TInstance> repository = CreateSagaRepository<TInstance>(container);

            configurator.StateMachineSaga(stateMachine, repository, configure);
        }

        public static ConnectHandle ConnectStateMachineSaga<TInstance>(this IConsumePipeConnector bus, SagaStateMachine<TInstance> stateMachine,
            Container container, Action<ISagaConfigurator<TInstance>> configure = null)
            where TInstance : class, SagaStateMachineInstance
        {
            ISagaRepository<TInstance> repository = CreateSagaRepository<TInstance>(container);

            return bus.ConnectStateMachineSaga(stateMachine, repository, configure);
        }

        public static ConnectHandle ConnectStateMachineSaga<TInstance>(this IConsumePipeConnector bus, Container container,
            Action<ISagaConfigurator<TInstance>> configure = null)
            where TInstance : class, SagaStateMachineInstance
        {
            var stateMachine = container.GetInstance<SagaStateMachine<TInstance>>();

            ISagaRepository<TInstance> repository = CreateSagaRepository<TInstance>(container);

            return bus.ConnectStateMachineSaga(stateMachine, repository, configure);
        }

        static ISagaRepository<TInstance> CreateSagaRepository<TInstance>(Container container)
            where TInstance : class, SagaStateMachineInstance
        {
            ISagaRepositoryFactory repositoryFactory = new SimpleInjectorSagaRepositoryFactory(container);

            return repositoryFactory.CreateSagaRepository<TInstance>(AddStateMachineActivityFactory);
        }

        static void AddStateMachineActivityFactory(ConsumeContext context)
        {
            context.GetOrAddPayload(() => _activityFactory);
        }
    }
}
