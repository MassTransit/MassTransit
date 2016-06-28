namespace MassTransit.RabbitMqTransport.Configuration
{
    public interface IRabbitMqClusterConfigurator
    {
        string[] ClusterMembers { get; set; }

        void Node(string clusterNodeHostname);
    }
}