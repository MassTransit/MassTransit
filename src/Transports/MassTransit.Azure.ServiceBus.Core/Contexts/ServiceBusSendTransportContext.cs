namespace MassTransit.Azure.ServiceBus.Core.Contexts
{
    using System;
    using Context;
    using Pipeline;


    public interface ServiceBusSendTransportContext :
        SendTransportContext
    {
        Uri Address { get; }

        ISendEndpointContextSupervisor Supervisor { get; }
    }
}
