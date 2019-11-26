namespace MassTransit.AmazonSqsTransport.Contexts
{
    using System;
    using Context;
    using GreenPipes;
    using Transport;


    public class HostSqsSendTransportContext :
        BaseSendTransportContext,
        SqsSendTransportContext
    {
        public HostSqsSendTransportContext(IClientContextSupervisor clientContextSupervisor, IPipe<ClientContext> configureTopologyPipe, string entityName,
            Func<string, bool> copyHeaderToMessageAttributesFilter, ILogContext logContext)
            : base(logContext)
        {
            ClientContextSupervisor = clientContextSupervisor;
            ConfigureTopologyPipe = configureTopologyPipe;
            EntityName = entityName;
            CopyHeaderToMessageAttributesFilter = copyHeaderToMessageAttributesFilter;
        }

        public IPipe<ClientContext> ConfigureTopologyPipe { get; }
        public string EntityName { get; }
        public Func<string, bool> CopyHeaderToMessageAttributesFilter { get; }
        public IClientContextSupervisor ClientContextSupervisor { get; }
    }
}
