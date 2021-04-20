using MassTransit.Transports;

namespace MassTransit.EventStoreDbIntegration.Contexts
{
    public interface IProducerContextSupervisor :
        ITransportSupervisor<ProducerContext>
    {
    }
}
