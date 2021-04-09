using System;
using System.Collections.Generic;
using System.Text;
using EventStore.Client;
using GreenPipes.Agents;
using MassTransit.Context;
using MassTransit.Pipeline;
using MassTransit.Pipeline.Observables;

namespace MassTransit.EventStoreDbIntegration.Contexts
{
    public interface IEventStoreDbProducerSharedContext :
        IAgent
    {
        Uri HostAddress { get; }
        ILogContext LogContext { get; }
        SendObservable SendObservers { get; }
        ISendPipe SendPipe { get; }
        IMessageSerializer Serializer { get; }
        EventStoreClient CreateEventStoreDbClient();
    }
}
