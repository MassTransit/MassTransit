using System;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using GreenPipes;
using MassTransit.EventStoreDbIntegration.Serializers;

namespace MassTransit.EventStoreDbIntegration.Contexts
{
    public class SharedSubscriptionContext :
        ProxyPipeContext,
        SubscriptionContext
    {
        readonly SubscriptionContext _context;

        public SharedSubscriptionContext(SubscriptionContext context, CancellationToken cancellationToken)
            : base(context)
        {
            _context = context;
            CancellationToken = cancellationToken;
        }

        public override CancellationToken CancellationToken { get; }

        public event Func<StreamSubscription, ResolvedEvent, CancellationToken, Task> ProcessEvent
        {
            add => _context.ProcessEvent += value;
            remove => _context.ProcessEvent -= value;
        }
        public event Action<StreamSubscription, SubscriptionDroppedReason, Exception> ProcessSubscriptionDropped
        {
            add => _context.ProcessSubscriptionDropped += value;
            remove => _context.ProcessSubscriptionDropped -= value;
        }

        public SubscriptionSettings SubscriptionSettings => _context.SubscriptionSettings;
        public IHeadersDeserializer HeadersDeserializer => _context.HeadersDeserializer;
        public ICheckpointStore CheckpointStore => _context.CheckpointStore;
        
        public Task SubscribeAsync(CancellationToken cancellationToken = default)
        {
            return _context.SubscribeAsync(cancellationToken);
        }

        public Task CloseAsync(CancellationToken cancellationToken = default)
        {
            return _context.CloseAsync(cancellationToken);
        }

        public Task Complete(ResolvedEvent resolvedEvent)
        {
            return _context.Complete(resolvedEvent);
        }

        public Task CheckpointReached(StreamSubscription streamSubscription, Position position, CancellationToken cancellationToken)
        {
            return _context.CheckpointReached(streamSubscription, position, cancellationToken);
        }
    }
}
