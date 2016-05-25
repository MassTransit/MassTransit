namespace MassTransit.RabbitMqTransport.Configuration.Configurators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class RabbitMqClusterConfigurator : IRabbitMqClusterConfigurator
    {
        private List<string> _clusterMembers;

        public RabbitMqClusterConfigurator()
        {
            _clusterMembers = new List<string>();
        }

        public string[] ClusterMembers
        {
            get
            {
                return _clusterMembers.ToArray();
            }
            set
            {
                _clusterMembers = value.ToList();
            }
        }

        public void Node(string clusterNodeHostname)
        {
            if (string.IsNullOrWhiteSpace(clusterNodeHostname))
            {
                throw new ArgumentException("Cluster node hostname cannot be empty.");
            }
            _clusterMembers.Add(clusterNodeHostname);
        }
    }
}
