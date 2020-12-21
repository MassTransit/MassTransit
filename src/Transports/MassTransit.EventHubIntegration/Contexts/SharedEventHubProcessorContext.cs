namespace MassTransit.EventHubIntegration.Contexts
{
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs;
    using Azure.Messaging.EventHubs.Processor;
    using GreenPipes;


    public class SharedEventHubProcessorContext :
        ProxyPipeContext,
        IEventHubProcessorContext
    {
        readonly IEventHubProcessorContext _context;

        public SharedEventHubProcessorContext(IEventHubProcessorContext context, CancellationToken cancellationToken)
            : base(context)
        {
            _context = context;
            CancellationToken = cancellationToken;
        }

        public override CancellationToken CancellationToken { get; }

        public IHostSettings HostSettings => _context.HostSettings;

        public IStorageSettings StorageSettings => _context.StorageSettings;

        public ReceiveSettings ReceiveSettings => _context.ReceiveSettings;

        public Task InitializePartition(PartitionInitializingEventArgs eventArgs)
        {
            return _context.InitializePartition(eventArgs);
        }

        public Task ClosePartition(PartitionClosingEventArgs eventArgs)
        {
            return _context.ClosePartition(eventArgs);
        }

        public Task<bool> TryCreateContainerIfNotExists(CancellationToken cancellationToken)
        {
            return _context.TryCreateContainerIfNotExists(cancellationToken);
        }

        public EventProcessorClient CreateClient()
        {
            return _context.CreateClient();
        }
    }
}
