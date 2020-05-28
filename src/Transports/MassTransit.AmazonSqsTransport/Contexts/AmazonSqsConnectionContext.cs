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
        readonly IAmazonSqsHostConfiguration _hostConfiguration;

        public AmazonSqsConnectionContext(IConnection connection, IAmazonSqsHostConfiguration hostConfiguration, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _hostConfiguration = hostConfiguration;
            Connection = connection;

            Topology = hostConfiguration.HostTopology;
        }

        public IConnection Connection { get; }
        public IAmazonSqsHostTopology Topology { get; }

        public Uri HostAddress => _hostConfiguration.HostAddress;

        public ClientContext CreateClientContext(CancellationToken cancellationToken)
        {
            var amazonSqs = Connection.CreateAmazonSqsClient();
            var amazonSns = Connection.CreateAmazonSnsClient();

            return new AmazonSqsClientContext(this, amazonSqs, amazonSns, cancellationToken);
        }
    }
}
