namespace MassTransit.RabbitMqTransport.Topology.Entities
{
    using MassTransit.Topology.Entities;


    public interface ExchangeBindingHandle :
        EntityHandle
    {
        ExchangeToExchangeBinding Binding { get; }
    }
}
