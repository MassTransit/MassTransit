namespace MassTransit.RabbitMqTransport.Topology
{
    using MassTransit.Topology;


    public interface QueueBindingHandle :
        EntityHandle
    {
        ExchangeToQueueBinding Binding { get; }
    }
}
