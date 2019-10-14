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
        readonly CancellationToken _cancellationToken;
        readonly ConnectionContext _context;

        public SharedConnectionContext(ConnectionContext context, CancellationToken cancellationToken)
            : base(context)
        {
            _context = context;
            _cancellationToken = cancellationToken;
        }

        CancellationToken PipeContext.CancellationToken => _cancellationToken;

        IConnection ConnectionContext.Connection => _context.Connection;
        public string Description => _context.Description;
        public Uri HostAddress => _context.HostAddress;
        bool ConnectionContext.PublisherConfirmation => _context.PublisherConfirmation;
        TimeSpan ConnectionContext.StopTimeout => _context.StopTimeout;
        public IRabbitMqHostTopology Topology => _context.Topology;

        Task<IModel> ConnectionContext.CreateModel(CancellationToken cancellationToken)
        {
            return _context.CreateModel(cancellationToken);
        }

        Task<ModelContext> ConnectionContext.CreateModelContext(CancellationToken cancellationToken)
        {
            return _context.CreateModelContext(cancellationToken);
        }
    }
}
