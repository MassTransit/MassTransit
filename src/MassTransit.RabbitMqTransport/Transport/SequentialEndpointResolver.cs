namespace MassTransit.RabbitMqTransport.Transport
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Context;
    using RabbitMQ.Client;


    public class SequentialEndpointResolver :
        IRabbitMqEndpointResolver
    {
        readonly ClusterNode[] _nodes;
        ClusterNode _lastNode;
        int _nextHostIndex;

        public SequentialEndpointResolver(ClusterNode[] nodes)
        {
            if (nodes == null)
                throw new ArgumentNullException(nameof(nodes));
            if (nodes.Length == 0)
                throw new ArgumentException("At least one cluster node must be specified", nameof(nodes));

            _nodes = nodes;
            _nextHostIndex = 0;
        }

        public ClusterNode LastHost => _lastNode;

        public IEnumerable<AmqpTcpEndpoint> All()
        {
            _lastNode = _nodes[_nextHostIndex % _nodes.Length];

            LogContext.Debug?.Log("Returning next host: {Host}", _lastNode);

            Interlocked.Increment(ref _nextHostIndex);

            yield return new AmqpTcpEndpoint(_lastNode.HostName, _lastNode.Port);
        }
    }
}
