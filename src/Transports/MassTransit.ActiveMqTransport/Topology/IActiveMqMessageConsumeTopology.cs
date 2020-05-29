namespace MassTransit.ActiveMqTransport.Topology
{
    using MassTransit.Topology;


    public interface IActiveMqMessageConsumeTopology<TMessage> :
        IMessageConsumeTopology<TMessage>
        where TMessage : class
    {
    }
}
