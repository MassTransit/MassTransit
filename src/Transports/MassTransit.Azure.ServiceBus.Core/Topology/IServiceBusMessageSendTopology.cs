namespace MassTransit.Azure.ServiceBus.Core.Topology
{
    using MassTransit.Topology;


    public interface IServiceBusMessageSendTopology<TMessage> :
        IMessageSendTopology<TMessage>
        where TMessage : class
    {
    }
}
