namespace MassTransit.EventHubIntegration.Contexts
{
    using System;
    using Azure.Messaging.EventHubs.Producer;
    using Context;
    using GreenPipes.Agents;
    using Pipeline;
    using Pipeline.Observables;


    public interface IEventHubProducerSharedContext:
        IAgent
    {
        Uri HostAddress { get; }
        ILogContext LogContext { get; }
        SendObservable SendObservers { get; }
        ISendPipe SendPipe { get; }
        IMessageSerializer Serializer { get; }
        EventHubProducerClient CreateEventHubClient(string eventHubName);
    }
}
