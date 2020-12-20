namespace MassTransit.EventHubIntegration.Contexts
{
    using Transports;


    public interface IProducerContextSupervisor :
        ITransportSupervisor<ProducerContext>
    {
    }
}
