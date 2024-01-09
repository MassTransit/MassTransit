namespace MassTransit.SqlTransport.Configuration
{
    using MassTransit.Configuration;


    public class SqlEndpointConfiguration :
        EndpointConfiguration,
        ISqlEndpointConfiguration
    {
        public SqlEndpointConfiguration(ISqlTopologyConfiguration topologyConfiguration)
            : base(topologyConfiguration)
        {
            Topology = topologyConfiguration;
        }

        SqlEndpointConfiguration(IEndpointConfiguration parentConfiguration, ISqlTopologyConfiguration topologyConfiguration, bool isBusEndpoint)
            : base(parentConfiguration, topologyConfiguration, isBusEndpoint)
        {
            Topology = topologyConfiguration;
        }

        public new ISqlTopologyConfiguration Topology { get; }

        public ISqlEndpointConfiguration CreateEndpointConfiguration(bool isBusEndpoint)
        {
            var topologyConfiguration = new SqlTopologyConfiguration(Topology);

            return new SqlEndpointConfiguration(this, topologyConfiguration, isBusEndpoint);
        }
    }
}
