namespace MassTransit.AzureServiceBusTransport.Contexts
{
    using System;
    using Context;
    using Pipeline;


    public interface ServiceBusSendTransportContext :
        SendTransportContext
    {
        Uri Address { get; }

        ISendEndpointContextSupervisor Source { get; }
    }
}
