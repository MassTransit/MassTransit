namespace MassTransit.GrpcTransport.Configuration
{
    using MassTransit.Configuration;
    using Topology;


    public interface IRoutingKeyMessageSendTopologyConvention<TMessage> :
        IMessageSendTopologyConvention<TMessage>
        where TMessage : class
    {
        void SetFormatter(IRoutingKeyFormatter formatter);
        void SetFormatter(IMessageRoutingKeyFormatter<TMessage> formatter);
    }
}
