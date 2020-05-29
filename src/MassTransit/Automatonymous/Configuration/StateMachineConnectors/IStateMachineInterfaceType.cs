namespace Automatonymous.StateMachineConnectors
{
    using MassTransit.Saga;
    using MassTransit.Saga.Connectors;


    public interface IStateMachineInterfaceType
    {
        ISagaMessageConnector<T> GetConnector<T>()
            where T : class, ISaga;
    }
}
