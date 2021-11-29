namespace MassTransit.RabbitMqTransport.Topology
{
    using MassTransit.Topology;


    public interface ExchangeHandle :
        EntityHandle
    {
        Exchange Exchange { get; }
    }
}
