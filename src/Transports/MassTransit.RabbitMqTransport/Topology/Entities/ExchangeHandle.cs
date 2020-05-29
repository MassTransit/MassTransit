namespace MassTransit.RabbitMqTransport.Topology.Entities
{
    using MassTransit.Topology.Entities;


    public interface ExchangeHandle :
        EntityHandle
    {
        Exchange Exchange { get; }
    }
}
