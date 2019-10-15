namespace MassTransit.AmazonSqsTransport.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.SimpleNotificationService;
    using Amazon.SQS;
    using GreenPipes;
    using Topology;
    using Transport;


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
        public IAmazonSqsHostTopology Topology => _context.Topology;

        Task<IAmazonSQS> ConnectionContext.CreateAmazonSqs()
        {
            return _context.CreateAmazonSqs();
        }

        Task<IAmazonSimpleNotificationService> ConnectionContext.CreateAmazonSns()
        {
            return _context.CreateAmazonSns();
        }
    }
}
