namespace MassTransit.EventHubIntegration
{
    using Transports;


    public class ProducerContextSupervisor :
        TransportPipeContextSupervisor<ProducerContext>,
        IProducerContextSupervisor
    {
        public ProducerContextSupervisor(IConnectionContextSupervisor contextSupervisor, string eventHubName)
            : base(new ProducerContextFactory(contextSupervisor, eventHubName))
        {
            contextSupervisor.AddSendAgent(this);
        }
    }
}
