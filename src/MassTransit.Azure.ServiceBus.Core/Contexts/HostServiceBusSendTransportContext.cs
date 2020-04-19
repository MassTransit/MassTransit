namespace MassTransit.Azure.ServiceBus.Core.Contexts
{
    using System;
    using Context;
    using Pipeline;


    public class HostServiceBusSendTransportContext :
        BaseSendTransportContext,
        ServiceBusSendTransportContext
    {
        public HostServiceBusSendTransportContext(Uri address, ISendEndpointContextSupervisor source, ILogContext logContext)
            : base(logContext)
        {
            Address = address;
            Supervisor = source;
        }

        public Uri Address { get; }
        public ISendEndpointContextSupervisor Supervisor { get; }
    }
}
