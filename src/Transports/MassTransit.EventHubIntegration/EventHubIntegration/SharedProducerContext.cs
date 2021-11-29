namespace MassTransit.EventHubIntegration
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs;
    using Azure.Messaging.EventHubs.Producer;
    using MassTransit.Middleware;


    public class SharedProducerContext :
        ProxyPipeContext,
        ProducerContext
    {
        readonly ProducerContext _context;

        public SharedProducerContext(ProducerContext context, CancellationToken cancellationToken)
            : base(context)
        {
            _context = context;
            CancellationToken = cancellationToken;
        }

        public override CancellationToken CancellationToken { get; }

        public ISerialization Serializer => _context.Serializer;

        public Task Produce(EventDataBatch eventDataBatch, CancellationToken cancellationToken)
        {
            return _context.Produce(eventDataBatch, cancellationToken);
        }

        public Task Produce(IEnumerable<EventData> eventData, SendEventOptions options, CancellationToken cancellationToken)
        {
            return _context.Produce(eventData, options, cancellationToken);
        }

        public ValueTask<EventDataBatch> CreateBatch(CreateBatchOptions options, CancellationToken cancellationToken)
        {
            return _context.CreateBatch(options, cancellationToken);
        }

        public ValueTask DisposeAsync()
        {
            return _context.DisposeAsync();
        }
    }
}
