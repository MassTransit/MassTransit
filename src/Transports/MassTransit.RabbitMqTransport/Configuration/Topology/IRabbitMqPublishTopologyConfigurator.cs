namespace MassTransit
{
    using System;


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

        new IRabbitMqMessagePublishTopologyConfigurator GetMessageTopology(Type messageType);
    }
}
