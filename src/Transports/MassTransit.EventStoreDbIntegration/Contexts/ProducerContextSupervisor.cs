using MassTransit.Transports;

namespace MassTransit.EventStoreDbIntegration.Contexts
{
    public class ProducerContextSupervisor :
        TransportPipeContextSupervisor<ProducerContext>,
        IProducerContextSupervisor
    {
        public ProducerContextSupervisor(IConnectionContextSupervisor contextSupervisor, StreamCategory streamCategory, IMessageSerializer messageSerializer)
            : base(new ProducerContextFactory(contextSupervisor, streamCategory, messageSerializer))
        {
            contextSupervisor.AddSendAgent(this);
        }
    }
}
