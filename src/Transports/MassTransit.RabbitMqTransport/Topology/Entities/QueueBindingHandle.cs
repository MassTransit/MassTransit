namespace MassTransit.RabbitMqTransport.Topology.Entities
{
    using MassTransit.Topology.Entities;


    public interface QueueBindingHandle :
        EntityHandle
    {
        ExchangeToQueueBinding Binding { get; }
    }
}
