namespace MassTransit.Azure.ServiceBus.Core.Contexts
{
    using System;
    using Context;
    using GreenPipes;


    public class HostServiceBusSendTransportContext :
        BaseSendTransportContext,
        ServiceBusSendTransportContext
    {
        public HostServiceBusSendTransportContext(Uri address, IPipeContextSource<SendEndpointContext> source, ILogContext logContext)
            : base(logContext)
        {
            Address = address;
            Source = source;
        }

        public Uri Address { get; }
        public IPipeContextSource<SendEndpointContext> Source { get; }
    }
}
