namespace MassTransit.RabbitMqTransport.Topology
{
    using MassTransit.Topology;


    public interface QueueHandle :
        EntityHandle
    {
        Queue Queue { get; }
    }
}
