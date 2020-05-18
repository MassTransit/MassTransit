namespace MassTransit.AmazonSqsTransport.Contexts
{
    using System;
    using System.Threading;
    using Configuration;
    using GreenPipes;
    using Topology;
    using Transport;


    public class AmazonSqsConnectionContext :
        BasePipeContext,
        ConnectionContext
    {
        readonly IAmazonSqsHostConfiguration _configuration;

        public AmazonSqsConnectionContext(IConnection connection, IAmazonSqsHostConfiguration configuration, IAmazonSqsHostTopology hostTopology,
            CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _configuration = configuration;
            Topology = hostTopology;
            Connection = connection;
        }

        public IConnection Connection { get; }
        public IAmazonSqsHostTopology Topology { get; }

        public Uri HostAddress => _configuration.HostAddress;

        public ClientContext CreateClientContext(CancellationToken cancellationToken)
        {
            var amazonSqs = Connection.CreateAmazonSqsClient();
            var amazonSns = Connection.CreateAmazonSnsClient();

            return new AmazonSqsClientContext(this, amazonSqs, amazonSns, cancellationToken);
        }
    }
}
