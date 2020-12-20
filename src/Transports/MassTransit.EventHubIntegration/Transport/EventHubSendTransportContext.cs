namespace MassTransit.EventHubIntegration
{
    using System;
    using Context;
    using Contexts;
    using Pipeline;


    public interface EventHubSendTransportContext :
        SendTransportContext
    {
        Uri HostAddress { get; }
        EventHubEndpointAddress EndpointAddress { get; }
        ISendPipe SendPipe { get; }
        IProducerContextSupervisor ProducerContextSupervisor { get; }
    }
}
