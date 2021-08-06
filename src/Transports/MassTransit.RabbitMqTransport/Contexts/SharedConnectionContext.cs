namespace MassTransit.RabbitMqTransport.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using RabbitMQ.Client;
    using Topology;


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

        public IConnection Connection => _context.Connection;
        public string Description => _context.Description;
        public Uri HostAddress => _context.HostAddress;
        public bool PublisherConfirmation => _context.PublisherConfirmation;
        public BatchSettings BatchSettings => _context.BatchSettings;
        public TimeSpan ContinuationTimeout => _context.ContinuationTimeout;

        public TimeSpan StopTimeout => _context.StopTimeout;
        public IRabbitMqHostTopology Topology => _context.Topology;

        public Task<IModel> CreateModel(CancellationToken cancellationToken)
        {
            return _context.CreateModel(cancellationToken);
        }

        public Task<ModelContext> CreateModelContext(CancellationToken cancellationToken)
        {
            return _context.CreateModelContext(cancellationToken);
        }
    }
}
