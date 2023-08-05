namespace MassTransit
{
    using KafkaIntegration;
    using Transports;


    public interface IKafkaRider :
        IRiderControl,
        ITopicProducerProvider,
        IKafkaTopicEndpointConnector
    {
    }
}
