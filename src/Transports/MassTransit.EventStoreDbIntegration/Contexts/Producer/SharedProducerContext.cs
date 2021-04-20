using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using GreenPipes;
using MassTransit.EventStoreDbIntegration.Serializers;

namespace MassTransit.EventStoreDbIntegration.Contexts
{
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

        public IHeadersSerializer HeadersSerializer => _context.HeadersSerializer;
        public IMessageSerializer Serializer => _context.Serializer;
        
        public Task Produce(string streamName, IEnumerable<EventData> eventData, CancellationToken cancellationToken)
        {
            return _context.Produce(streamName, eventData, cancellationToken);
        }

        public ValueTask DisposeAsync()
        {
            return _context.DisposeAsync();
        }
    }
}
