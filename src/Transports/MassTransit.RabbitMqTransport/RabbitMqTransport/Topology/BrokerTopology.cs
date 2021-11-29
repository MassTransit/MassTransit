namespace MassTransit.RabbitMqTransport.Topology
{
    public interface BrokerTopology :
        IProbeSite
    {
        Exchange[] Exchanges { get; }
        Queue[] Queues { get; }
        ExchangeToExchangeBinding[] ExchangeBindings { get; }
        ExchangeToQueueBinding[] QueueBindings { get; }
    }
}
