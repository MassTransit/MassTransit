namespace MassTransit.EventHubIntegration
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs;
    using Azure.Messaging.EventHubs.Producer;
    using GreenPipes;


    public class EventHubProducerContext :
        BasePipeContext,
        ProducerContext
    {
        readonly EventHubProducerClient _producerClient;

        public EventHubProducerContext(EventHubProducerClient producerClient, IMessageSerializer messageSerializer,
            CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _producerClient = producerClient;
            Serializer = messageSerializer;
        }

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
