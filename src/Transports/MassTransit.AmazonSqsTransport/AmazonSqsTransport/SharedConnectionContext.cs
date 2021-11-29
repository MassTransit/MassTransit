namespace MassTransit.AmazonSqsTransport
{
    using System;
    using System.Threading;
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

        IConnection ConnectionContext.Connection => _context.Connection;
        public Uri HostAddress => _context.HostAddress;
        public IAmazonSqsBusTopology Topology => _context.Topology;

        public ClientContext CreateClientContext(CancellationToken cancellationToken)
        {
            return _context.CreateClientContext(cancellationToken);
        }
    }
}
