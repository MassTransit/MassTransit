namespace MassTransit.ActiveMqTransport.Contexts
{
    using Apache.NMS;
    using Context;
    using GreenPipes;
    using Transport;


    public class HostActiveMqSendTransportContext :
        BaseSendTransportContext,
        ActiveMqSendTransportContext
    {
        public HostActiveMqSendTransportContext(ISessionContextSupervisor sessionContextSupervisor, IPipe<SessionContext> configureTopologyPipe, string
            entityName, DestinationType destinationType, ILogContext logContext)
            : base(logContext)
        {
            SessionContextSupervisor = sessionContextSupervisor;
            ConfigureTopologyPipe = configureTopologyPipe;
            EntityName = entityName;
            DestinationType = destinationType;
        }

        public IPipe<SessionContext> ConfigureTopologyPipe { get; }
        public string EntityName { get; }
        public DestinationType DestinationType { get; }
        public ISessionContextSupervisor SessionContextSupervisor { get; }
    }
}
