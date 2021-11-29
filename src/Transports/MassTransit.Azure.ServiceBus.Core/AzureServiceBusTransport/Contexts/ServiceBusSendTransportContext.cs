namespace MassTransit.AzureServiceBusTransport
{
    using System;
    using Context;
    using Transports;


    public interface ServiceBusSendTransportContext :
        SendTransportContext,
        IPipeContextSource<SendEndpointContext>
    {
        Uri Address { get; }
    }
}
