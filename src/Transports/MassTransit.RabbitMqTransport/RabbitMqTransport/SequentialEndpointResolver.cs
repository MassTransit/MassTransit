namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Configuration;
    using RabbitMQ.Client;


    public class SequentialEndpointResolver :
        IRabbitMqEndpointResolver
    {
        readonly ClusterNode[] _nodes;
        readonly RabbitMqHostSettings _settings;
        ClusterNode _lastNode;
        int _nextHostIndex;

        public SequentialEndpointResolver(ClusterNode[] nodes, RabbitMqHostSettings settings)
        {
            if (nodes == null)
                throw new ArgumentNullException(nameof(nodes));
            if (nodes.Length == 0)
                throw new ArgumentException("At least one cluster node must be specified", nameof(nodes));

            _nodes = nodes;
            _settings = settings;
            _nextHostIndex = 0;
        }

        public ClusterNode LastHost => _lastNode;

        public IEnumerable<AmqpTcpEndpoint> All()
        {
            _lastNode = _nodes[_nextHostIndex % _nodes.Length];

            var hostName = _lastNode.HostName;
            var port = _lastNode.Port ?? _settings.Port;

            Interlocked.Increment(ref _nextHostIndex);

            LogContext.Debug?.Log("Returning next host: {Host}:{Port}", hostName, port);

            var endpoint = new AmqpTcpEndpoint(hostName, port);

            _settings.ApplySslOptions(endpoint.Ssl);

            yield return endpoint;
        }
    }
}
