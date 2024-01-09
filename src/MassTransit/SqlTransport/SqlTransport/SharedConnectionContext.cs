#nullable enable
namespace MassTransit.SqlTransport
{
    using System;
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;
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

        public Uri HostAddress => _context.HostAddress;
        public string? Schema => _context.Schema;
        public IsolationLevel IsolationLevel => _context.IsolationLevel;

        public ClientContext CreateClientContext(CancellationToken cancellationToken)
        {
            return _context.CreateClientContext(cancellationToken);
        }

        public Task<ISqlTransportConnection> CreateConnection(CancellationToken cancellationToken)
        {
            return _context.CreateConnection(cancellationToken);
        }

        public Task DelayUntilMessageReady(long queueId, TimeSpan timeout, CancellationToken cancellationToken)
        {
            return _context.DelayUntilMessageReady(queueId, timeout, cancellationToken);
        }

        public Task<T> Query<T>(Func<IDbConnection, IDbTransaction, Task<T>> callback, CancellationToken cancellationToken)
        {
            return _context.Query(callback, cancellationToken);
        }
    }
}
