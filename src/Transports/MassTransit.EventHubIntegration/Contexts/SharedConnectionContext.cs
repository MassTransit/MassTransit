namespace MassTransit.EventHubIntegration.Contexts
{
    using System.Threading;
    using System.Threading.Tasks;
    using Azure.Messaging.EventHubs.Producer;
    using GreenPipes;


    public class SharedConnectionContext :
        ProxyPipeContext,
        ConnectionContext
    {
        readonly ConnectionContext _context;

        public SharedConnectionContext(ConnectionContext context, CancellationToken cancellationToken)
            : base(context)
        {
            _context = context;
            CancellationToken = cancellationToken;
        }

        public override CancellationToken CancellationToken { get; }

        public IHostSettings HostSettings => _context.HostSettings;

        public IStorageSettings StorageSettings => _context.StorageSettings;

        public EventHubProducerClient CreateEventHubClient(string eventHubName)
        {
            return _context.CreateEventHubClient(eventHubName);
        }

        public ValueTask DisposeAsync()
        {
            return _context.DisposeAsync();
        }
    }
}
