using System;
using MassTransit.Context;
using MassTransit.EventStoreDbIntegration.Contexts;
using MassTransit.Pipeline;

namespace MassTransit.EventStoreDbIntegration
{
    public interface EventStoreDbSendTransportContext :
        SendTransportContext
    {
        Uri HostAddress { get; }
        EventStoreDbEndpointAddress EndpointAddress { get; }
        ISendPipe SendPipe { get; }
        IProducerContextSupervisor ProducerContextSupervisor { get; }
    }
}
