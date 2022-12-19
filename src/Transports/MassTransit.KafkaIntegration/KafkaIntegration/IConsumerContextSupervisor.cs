namespace MassTransit.KafkaIntegration
{
    using Transports;


    public interface IConsumerContextSupervisor :
        ITransportSupervisor<ConsumerContext>
    {
    }
}
