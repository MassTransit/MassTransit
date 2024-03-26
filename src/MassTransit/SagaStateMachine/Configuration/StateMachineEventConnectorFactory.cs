namespace MassTransit.Configuration
{
    using System;
    using Middleware;


    public partial class StateMachineInterfaceType<TInstance, TData>
    {
        public class StateMachineEventConnectorFactory :
            ISagaConnectorFactory
        {
            readonly ISagaMessageConnector<TInstance> _connector;

            public StateMachineEventConnectorFactory(SagaStateMachine<TInstance> stateMachine, EventCorrelation<TInstance, TData> correlation)
            {
                var consumeFilter = new StateMachineSagaMessageFilter<TInstance, TData>(stateMachine, correlation.Event);

                _connector = new StateMachineSagaMessageConnector(consumeFilter, correlation.Policy,
                    correlation.FilterFactory,
                    correlation.MessageFilter, correlation.ConfigureConsumeTopology);
            }

            ISagaMessageConnector<T> ISagaConnectorFactory.CreateMessageConnector<T>()
            {
                if (_connector is ISagaMessageConnector<T> connector)
                    return connector;

                throw new ArgumentException("The saga type did not match the connector type");
            }
        }
    }
}
