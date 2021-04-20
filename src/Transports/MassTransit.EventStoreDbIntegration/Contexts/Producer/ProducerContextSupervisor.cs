using MassTransit.EventStoreDbIntegration.Serializers;
using MassTransit.Transports;

namespace MassTransit.EventStoreDbIntegration.Contexts
{
    public class ProducerContextSupervisor :
        TransportPipeContextSupervisor<ProducerContext>,
        IProducerContextSupervisor
    {
        public ProducerContextSupervisor(IConnectionContextSupervisor contextSupervisor, IHeadersSerializer headersSerializer,
            IMessageSerializer messageSerializer)
            : base(new ProducerContextFactory(contextSupervisor, headersSerializer, messageSerializer))
        {
            contextSupervisor.AddSendAgent(this);
        }
    }
}
