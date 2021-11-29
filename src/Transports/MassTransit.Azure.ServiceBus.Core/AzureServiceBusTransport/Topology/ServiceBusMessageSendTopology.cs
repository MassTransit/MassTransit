namespace MassTransit.AzureServiceBusTransport.Topology
{
    using MassTransit.Topology;


    public class ServiceBusMessageSendTopology<TMessage> :
        MessageSendTopology<TMessage>,
        IServiceBusMessageSendTopologyConfigurator<TMessage>
        where TMessage : class
    {
    }
}
