namespace MassTransit.EventHubIntegration
{
    using System.Threading;
    using Azure.Messaging.EventHubs.Producer;
    using Configuration;
    using MassTransit.Middleware;


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
    }
}
