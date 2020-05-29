namespace MassTransit.RabbitMqTransport.Topology.Conventions
{
    using MassTransit.Topology;


    public interface IRoutingKeyMessageSendTopologyConvention<TMessage> :
        IMessageSendTopologyConvention<TMessage>
        where TMessage : class
    {
        void SetFormatter(IRoutingKeyFormatter formatter);
        void SetFormatter(IMessageRoutingKeyFormatter<TMessage> formatter);
    }
}
