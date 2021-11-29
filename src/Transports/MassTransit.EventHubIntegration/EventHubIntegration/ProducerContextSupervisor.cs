namespace MassTransit.EventHubIntegration
{
    using Transports;


    public class ProducerContextSupervisor :
        TransportPipeContextSupervisor<ProducerContext>,
        IProducerContextSupervisor
    {
        public ProducerContextSupervisor(IConnectionContextSupervisor contextSupervisor, string eventHubName, ISerialization serializers)
            : base(new ProducerContextFactory(contextSupervisor, eventHubName, serializers))
        {
            contextSupervisor.AddSendAgent(this);
        }
    }
}
