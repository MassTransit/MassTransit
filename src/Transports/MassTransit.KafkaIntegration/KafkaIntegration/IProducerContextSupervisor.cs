namespace MassTransit.KafkaIntegration
{
    using Transports;


    public interface IProducerContextSupervisor :
        ITransportSupervisor<ProducerContext>
    {
    }
}
