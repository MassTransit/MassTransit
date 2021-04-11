using System;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using GreenPipes;
using MassTransit.EventStoreDbIntegration.Serializers;

namespace MassTransit.EventStoreDbIntegration.Contexts
{
    public class SharedProcessorContext :
        ProxyPipeContext,
        ProcessorContext
    {
        readonly ProcessorContext _context;

        public SharedProcessorContext(ProcessorContext context, CancellationToken cancellationToken)
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

        public ReceiveSettings ReceiveSettings => _context.ReceiveSettings;
        public ICheckpointStore CheckpointStore => _context.CheckpointStore;
        public IHeadersDeserializer MetadataDeserializer => _context.MetadataDeserializer;

        public Task StartProcessingAsync(CancellationToken cancellationToken = default)
        {
            return _context.StartProcessingAsync(cancellationToken);
        }

        public Task StopProcessingAsync(CancellationToken cancellationToken = default)
        {
            return _context.StopProcessingAsync(cancellationToken);
        }

        public Task Complete(ResolvedEvent resolvedEvent)
        {
            return _context.Complete(resolvedEvent);
        }
    }
}
