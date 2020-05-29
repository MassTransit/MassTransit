namespace MassTransit.Azure.ServiceBus.Core.Topology.Topologies
{
    using MassTransit.Topology.Topologies;


    public class ServiceBusMessageSendTopology<TMessage> :
        MessageSendTopology<TMessage>,
        IServiceBusMessageSendTopologyConfigurator<TMessage>
        where TMessage : class
    {
    }
}
