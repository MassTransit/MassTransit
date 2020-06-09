namespace MassTransit.EventHubIntegration.Contexts
{
    using System;
    using Azure.Messaging.EventHubs.Producer;
    using Context;
    using Pipeline;
    using Pipeline.Observables;


    public interface IEventHubProducerSharedContext :
        IAsyncDisposable
    {
        Uri HostAddress { get; }
        ILogContext LogContext { get; }
        SendObservable SendObservers { get; }
        ISendPipe SendPipe { get; }
        IMessageSerializer Serializer { get; }
        EventHubProducerClient CreateEventHubClient(string eventHubName);
    }
}
