namespace MassTransit.EventHubIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs;
    using Azure.Messaging.EventHubs.Producer;
    using Context;
    using Contexts;
    using GreenPipes;
    using Pipeline;
    using Pipeline.Observables;


    public class ProducerProvider :
        IProducerProvider
    {
        readonly ConsumeContext _consumeContext;
        readonly IEventHubProducerSharedContext _context;

        public ProducerProvider(IEventHubProducerSharedContext context, ConsumeContext consumeContext)
        {
            _context = context;
            _consumeContext = consumeContext;
        }

        public IEventHubProducer GetProducer(Uri address)
        {
            var endpointAddress = new EventHubEndpointAddress(_context.HostAddress, address);
            var client = _context.CreateEventHubClient(endpointAddress.EventHubName);
            var context = new EventHubProducerContext(client, _context);
            return new EventHubProducer(endpointAddress, context, _consumeContext);
        }


        class EventHubProducerContext :
            IEventHubProducerContext
        {
            readonly ISendPipe _pipe;
            readonly EventHubProducerClient _producerClient;

            public EventHubProducerContext(EventHubProducerClient producerClient, IEventHubProducerSharedContext context)
            {
                _producerClient = producerClient;
                HostAddress = context.HostAddress;
                LogContext = context.LogContext;
                SendObservers = context.SendObservers;
                Serializer = context.Serializer;
                _pipe = context.SendPipe;
            }

            public Task Send<T>(SendContext<T> context)
                where T : class
            {
                return _pipe.Send(context);
            }

            public void Probe(ProbeContext context)
            {
                _pipe.Probe(context);
            }

            public Uri HostAddress { get; }
            public ILogContext LogContext { get; }
            public SendObservable SendObservers { get; }
            public IMessageSerializer Serializer { get; }

            public Task Produce(IEnumerable<EventData> eventData, SendEventOptions options, CancellationToken cancellationToken)
            {
                return _producerClient.SendAsync(eventData, options, cancellationToken);
            }

            public Task Produce(EventDataBatch eventDataBatch, CancellationToken cancellationToken)
            {
                return _producerClient.SendAsync(eventDataBatch, cancellationToken);
            }

            public ValueTask<EventDataBatch> CreateBatch(CreateBatchOptions options, CancellationToken cancellationToken)
            {
                return _producerClient.CreateBatchAsync(options, cancellationToken);
            }
        }
    }
}
