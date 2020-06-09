namespace MassTransit.EventHubIntegration
{
    using System;
    using Azure.Messaging.EventHubs.Producer;


    public interface IEventHubProducerConfigurator :
        ISendObserverConnector,
        ISendPipelineConfigurator
    {
        /// <summary>
        /// Configure <see cref="EventHubProducerClientOptions" />
        /// </summary>
        Action<EventHubProducerClientOptions> ConfigureOptions { set; }

        /// <summary>
        /// Sets the outbound message serializer
        /// </summary>
        /// <param name="serializerFactory">The factory to create the message serializer</param>
        void SetMessageSerializer(SerializerFactory serializerFactory);
    }
}
