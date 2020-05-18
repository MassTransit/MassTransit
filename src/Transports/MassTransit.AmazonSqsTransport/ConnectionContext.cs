namespace MassTransit.AmazonSqsTransport
{
    using System;
    using System.Threading;
    using GreenPipes;
    using Topology;
    using Transport;


    public interface ConnectionContext :
        PipeContext
    {
        /// <summary>
        /// The Amazon Connection
        /// </summary>
        IConnection Connection { get; }

        /// <summary>
        /// The Host Address for this connection
        /// </summary>
        Uri HostAddress { get; }

        IAmazonSqsHostTopology Topology { get; }

        ClientContext CreateClientContext(CancellationToken cancellationToken);
    }
}
