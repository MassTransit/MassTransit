namespace MassTransit.RabbitMqTransport.Configuration
{
    using System.Collections.Generic;


    public class RabbitMqClusterConfigurator :
        IRabbitMqClusterConfigurator
    {
        readonly List<ClusterNode> _nodes;
        readonly RabbitMqHostSettings _settings;

        public RabbitMqClusterConfigurator(RabbitMqHostSettings settings)
        {
            _settings = settings;
            _nodes = new List<ClusterNode>();
        }

        public ClusterNode[] ClusterMembers => _nodes.ToArray();

        public void Node(string nodeAddress)
        {
            _nodes.Add(ClusterNode.Parse(nodeAddress));
        }

        public IRabbitMqEndpointResolver GetEndpointResolver()
        {
            if (_nodes.Count <= 0)
                return null;

            return new SequentialEndpointResolver(ClusterMembers, _settings);
        }
    }
}
