namespace MassTransit.Azure.ServiceBus.Core.Topology
{
    using MassTransit.Topology;


    public interface IServiceBusMessageConsumeTopology<TMessage> :
        IMessageConsumeTopology<TMessage>
        where TMessage : class
    {
    }
}
