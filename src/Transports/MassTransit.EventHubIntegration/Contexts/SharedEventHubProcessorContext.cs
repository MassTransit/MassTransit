namespace MassTransit.EventHubIntegration.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs;
    using Azure.Messaging.EventHubs.Processor;
    using Azure.Storage.Blobs;
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

        public event Func<PartitionInitializingEventArgs, Task> PartitionInitializing
        {
            add => _context.PartitionInitializing += value;
            remove => _context.PartitionInitializing -= value;
        }

        public event Func<PartitionClosingEventArgs, Task> PartitionClosing
        {
            add => _context.PartitionClosing += value;
            remove => _context.PartitionClosing -= value;
        }

        public BlobContainerClient BlobContainerClient => _context.BlobContainerClient;

        public EventProcessorClient EventProcessorClient => _context.EventProcessorClient;

        public ReceiveSettings ReceiveSettings => _context.ReceiveSettings;
    }
}
