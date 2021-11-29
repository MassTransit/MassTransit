namespace MassTransit
{
    using System;
    using ActiveMqTransport;
    using ActiveMqTransport.Topology;


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
        void Bind(string topicName, Action<IActiveMqTopicBindingConfigurator> configure = null);
    }
}
