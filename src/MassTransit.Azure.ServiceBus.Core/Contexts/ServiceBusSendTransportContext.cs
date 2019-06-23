namespace MassTransit.Azure.ServiceBus.Core.Contexts
{
    using System;
    using Context;
    using GreenPipes;


    public interface ServiceBusSendTransportContext :
        SendTransportContext
    {
        Uri Address { get; }

        IPipeContextSource<SendEndpointContext> Source { get; }
    }
}
