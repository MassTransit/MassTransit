namespace MassTransit
{
    public interface IRabbitMqClusterConfigurator
    {
        /// <summary>
        /// Add a node to the cluster, which may include a host name, and an option port number.
        /// </summary>
        /// <param name="nodeAddress">The node address</param>
        void Node(string nodeAddress);
    }
}
