namespace MassTransit.Azure.ServiceBus.Core.Contexts
{
    using System;
    using Context;
    using GreenPipes;


    public interface ServiceBusSendTransportContext :
        SendTransportContext,
        IPipeContextSource<SendEndpointContext>
    {
        Uri Address { get; }
    }
}
