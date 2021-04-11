using MassTransit.Transports;

namespace MassTransit.EventStoreDbIntegration.Contexts
{
    public class ProducerContextSupervisor :
        TransportPipeContextSupervisor<ProducerContext>,
        IProducerContextSupervisor
    {
        public ProducerContextSupervisor(IClientContextSupervisor contextSupervisor, string streamName, IMessageSerializer messageSerializer)
            : base(new ProducerContextFactory(contextSupervisor, streamName, messageSerializer))
        {
            contextSupervisor.AddSendAgent(this);
        }
    }
}
