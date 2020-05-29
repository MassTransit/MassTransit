namespace MassTransit.RabbitMqTransport.Topology
{
    using MassTransit.Topology;


    public interface IRabbitMqPublishTopologyConfigurator :
        IPublishTopologyConfigurator,
        IRabbitMqPublishTopology
    {
        /// <summary>
        /// Determines how type hierarchy is configured on the broker
        /// </summary>
        new PublishBrokerTopologyOptions BrokerTopologyOptions { set; }

        new IRabbitMqMessagePublishTopologyConfigurator<T> GetMessageTopology<T>()
            where T : class;
    }
}
