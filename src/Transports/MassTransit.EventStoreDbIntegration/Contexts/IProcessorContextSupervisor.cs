using MassTransit.Transports;

namespace MassTransit.EventStoreDbIntegration.Contexts
{
    public interface IProcessorContextSupervisor :
        ITransportSupervisor<ProcessorContext>
    {
    }
}
