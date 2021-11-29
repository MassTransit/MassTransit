namespace MassTransit.ActiveMqTransport
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Apache.NMS;
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
        public string Description => _context.Description;
        public Uri HostAddress => _context.HostAddress;
        public IActiveMqBusTopology Topology => _context.Topology;

        Task<ISession> ConnectionContext.CreateSession(CancellationToken cancellationToken)
        {
            return _context.CreateSession(cancellationToken);
        }
    }
}
