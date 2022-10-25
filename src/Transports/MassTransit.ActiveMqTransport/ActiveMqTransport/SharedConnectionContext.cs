namespace MassTransit.ActiveMqTransport
{
    using Apache.NMS;
    using MassTransit.Middleware;
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;


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

        IConnection ConnectionContext.Connection => _context.Connection;
        public string Description => _context.Description;
        public Uri HostAddress => _context.HostAddress;
        public IActiveMqBusTopology Topology => _context.Topology;

        public ConcurrentDictionary<string, IDestination> TemporaryDestinationMap => _context.TemporaryDestinationMap;

        Task<ISession> ConnectionContext.CreateSession(CancellationToken cancellationToken)
        {
            return _context.CreateSession(cancellationToken);
        }
    }
}
