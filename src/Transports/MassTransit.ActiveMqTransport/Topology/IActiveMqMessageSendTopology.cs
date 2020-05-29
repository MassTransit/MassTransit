namespace MassTransit.ActiveMqTransport.Topology
{
    using MassTransit.Topology;


    public interface IActiveMqMessageSendTopology<TMessage> :
        IMessageSendTopology<TMessage>
        where TMessage : class
    {
    }
}
