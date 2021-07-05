namespace MassTransit.ActiveMqTransport.Topology
{
    using System;
    using Builders;
    using MassTransit.Topology;


    public interface IActiveMqConsumeTopology :
        IConsumeTopology
    {
        IActiveMqConsumerEndpointQueueNameFormatter ConsumerEndpointQueueNameFormatter { get; }

        IActiveMqTemporaryQueueNameFormatter TemporaryQueueNameFormatter { get; }

        new IActiveMqMessageConsumeTopology<T> GetMessageTopology<T>()
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
        void Bind(string topicName, Action<ITopicBindingConfigurator> configure = null);
    }
}
