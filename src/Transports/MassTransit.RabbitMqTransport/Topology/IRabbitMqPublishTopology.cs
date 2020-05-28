namespace MassTransit.RabbitMqTransport.Topology
{
    using MassTransit.Topology;


    public interface IRabbitMqPublishTopology :
        IPublishTopology
    {
        IExchangeTypeSelector ExchangeTypeSelector { get; }

        /// <summary>
        /// Determines how type hierarchy is configured on the broker
        /// </summary>
        PublishBrokerTopologyOptions BrokerTopologyOptions { get; }

        new IRabbitMqMessagePublishTopology<T> GetMessageTopology<T>()
            where T : class;
    }
}
