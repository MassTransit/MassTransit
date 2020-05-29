namespace MassTransit.Azure.ServiceBus.Core.Topology.Conventions
{
    using MassTransit.Topology;


    public interface IPartitionKeyMessageSendTopologyConvention<TMessage> :
        IMessageSendTopologyConvention<TMessage>
        where TMessage : class
    {
        void SetFormatter(IPartitionKeyFormatter formatter);
        void SetFormatter(IMessagePartitionKeyFormatter<TMessage> formatter);
    }
}
