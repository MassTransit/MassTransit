namespace MassTransit.Azure.ServiceBus.Core.Topology.Conventions
{
    using MassTransit.Topology;


    public interface ISessionIdMessageSendTopologyConvention<TMessage> :
        IMessageSendTopologyConvention<TMessage>
        where TMessage : class
    {
        void SetFormatter(ISessionIdFormatter formatter);
        void SetFormatter(IMessageSessionIdFormatter<TMessage> formatter);
    }
}
