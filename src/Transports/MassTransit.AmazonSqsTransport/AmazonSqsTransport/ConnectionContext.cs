namespace MassTransit.AmazonSqsTransport
{
    using System;
    using System.Threading;
    using MassTransit.Topology;
    using Topology;


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

        IAmazonSqsBusTopology Topology { get; }

        ClientContext CreateClientContext(CancellationToken cancellationToken);
    }
}
