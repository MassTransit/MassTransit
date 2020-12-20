namespace MassTransit.EventHubIntegration.Contexts
{
    using Transports;


    public class ProducerContextSupervisor :
        TransportPipeContextSupervisor<ProducerContext>,
        IProducerContextSupervisor
    {
        public ProducerContextSupervisor(IConnectionContextSupervisor contextSupervisor, string eventHubName, IMessageSerializer messageSerializer)
            : base(new ProducerContextFactory(contextSupervisor, eventHubName, messageSerializer))
        {
            contextSupervisor.AddSendAgent(this);
        }
    }
}
