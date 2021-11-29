namespace MassTransit
{
    using System;
    using AmazonSqsTransport.Topology;


    public interface IAmazonSqsConsumeTopology :
        IConsumeTopology
    {
        new IAmazonSqsMessageConsumeTopology<T> GetMessageTopology<T>()
            where T : class;

        /// <summary>
        /// Apply the entire topology to the builder
        /// </summary>
        /// <param name="builder"></param>
        void Apply(IReceiveEndpointBrokerTopologyBuilder builder);

        /// <summary>
        /// Bind an exchange, using the configurator
        /// </summary>
        /// <param name="topicName"></param>
        /// <param name="configure"></param>
        void Bind(string topicName, Action<IAmazonSqsTopicSubscriptionConfigurator> configure = null);
    }
}
