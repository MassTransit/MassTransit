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

        public bool IsVirtualTopicConsumer(string name)
        {
            return _context.IsVirtualTopicConsumer(name);
        }

        public IQueue GetTemporaryQueue(ISession session, string topicName)
        {
            return _context.GetTemporaryQueue(session, topicName);
        }

        public ITopic GetTemporaryTopic(ISession session, string topicName)
        {
            return _context.GetTemporaryTopic(session, topicName);
        }

        public bool TryGetTemporaryEntity(string name, out IDestination destination)
        {
            return _context.TryGetTemporaryEntity(name, out destination);
        }

        public bool TryRemoveTemporaryEntity(ISession session, string name)
        {
            return _context.TryRemoveTemporaryEntity(session, name);
        }
    }
}
