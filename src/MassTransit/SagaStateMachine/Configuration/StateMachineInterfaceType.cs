namespace MassTransit.Configuration
{
    public class StateMachineInterfaceType<TInstance, TData> :
        IStateMachineInterfaceType
        where TInstance : class, ISaga, SagaStateMachineInstance
        where TData : class
    {
        readonly ISagaConnectorFactory _connectorFactory;

        public StateMachineInterfaceType(SagaStateMachine<TInstance> machine, EventCorrelation<TInstance, TData> correlation)
        {
            _connectorFactory = new StateMachineEventConnectorFactory<TInstance, TData>(machine, correlation);
        }

        ISagaMessageConnector<T> IStateMachineInterfaceType.GetConnector<T>()
        {
            return _connectorFactory.CreateMessageConnector<T>();
        }
    }
}
