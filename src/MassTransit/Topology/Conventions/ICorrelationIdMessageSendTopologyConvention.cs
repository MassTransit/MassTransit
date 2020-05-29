namespace MassTransit.Topology.Conventions
{
    using Context;


    public interface ICorrelationIdMessageSendTopologyConvention<TMessage> :
        IMessageSendTopologyConvention<TMessage>
        where TMessage : class
    {
        void SetCorrelationId(ISetCorrelationId<TMessage> setCorrelationId);
    }
}
