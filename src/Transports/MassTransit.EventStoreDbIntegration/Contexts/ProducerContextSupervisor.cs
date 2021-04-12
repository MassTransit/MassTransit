using MassTransit.Transports;

namespace MassTransit.EventStoreDbIntegration.Contexts
{
    public class ProducerContextSupervisor :
        TransportPipeContextSupervisor<ProducerContext>,
        IProducerContextSupervisor
    {
        public ProducerContextSupervisor(IConnectionContextSupervisor contextSupervisor, IMessageSerializer messageSerializer)
            : base(new ProducerContextFactory(contextSupervisor, messageSerializer))
        {
            contextSupervisor.AddSendAgent(this);
        }
    }
}
