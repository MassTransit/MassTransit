namespace MassTransit.RabbitMqTransport.Topology
{
    using MassTransit.Topology;


    public interface ExchangeBindingHandle :
        EntityHandle
    {
        ExchangeToExchangeBinding Binding { get; }
    }
}
