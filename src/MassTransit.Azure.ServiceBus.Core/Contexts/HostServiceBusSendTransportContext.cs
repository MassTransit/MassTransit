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
            Source = source;
        }

        public Uri Address { get; }
        public ISendEndpointContextSupervisor Source { get; }
    }
}
