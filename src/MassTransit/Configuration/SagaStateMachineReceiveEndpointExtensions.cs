#nullable enable
namespace MassTransit
{
    using System;
    using Configuration;


    public static class SagaStateMachineReceiveEndpointExtensions
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
            ISagaRepository<TInstance> repository, Action<ISagaConfigurator<TInstance>>? configure = null)
            where TInstance : class, SagaStateMachineInstance
        {
            if (stateMachine == null)
                throw new ArgumentNullException(nameof(stateMachine));
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));
            var stateMachineConfigurator = new StateMachineSagaConfigurator<TInstance>(stateMachine, repository, configurator);

            configure?.Invoke(stateMachineConfigurator);

            configurator.AddEndpointSpecification(stateMachineConfigurator);
        }

        public static ConnectHandle ConnectStateMachineSaga<TInstance>(this IConsumePipeConnector bus, SagaStateMachine<TInstance> stateMachine,
            ISagaRepository<TInstance> repository, Action<ISagaConfigurator<TInstance>>? configure = null)
            where TInstance : class, SagaStateMachineInstance
        {
            var connector = new StateMachineConnector<TInstance>(stateMachine);

            ISagaSpecification<TInstance> specification = connector.CreateSagaSpecification<TInstance>();

            configure?.Invoke(specification);

            return connector.ConnectSaga(bus, repository, specification);
        }
    }
}
