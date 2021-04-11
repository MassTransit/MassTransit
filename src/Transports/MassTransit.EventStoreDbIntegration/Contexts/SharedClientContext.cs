using System.Threading;
using EventStore.Client;
using GreenPipes;

namespace MassTransit.EventStoreDbIntegration.Contexts
{
    public class SharedClientContext :
        ProxyPipeContext,
        ClientContext
    {

        readonly ClientContext _context;

        public SharedClientContext(ClientContext context, CancellationToken cancellationToken)
            : base(context)
        {
            _context = context;
            CancellationToken = cancellationToken;
        }

        public override CancellationToken CancellationToken { get; }

        public IHostSettings HostSettings => _context.HostSettings;

        public EventStoreClient CreateEventStoreDbClient() => _context.CreateEventStoreDbClient();
    }
}
