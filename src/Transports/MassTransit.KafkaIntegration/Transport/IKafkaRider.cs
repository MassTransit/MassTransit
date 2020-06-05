namespace MassTransit.KafkaIntegration.Transport
{
    using Riders;


    public interface IKafkaRider :
        IRider,
        IKafkaProducerProvider
    {
    }
}
