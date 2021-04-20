namespace MassTransit.GrpcTransport.Configuration
{
    using MassTransit.Configuration;


    public class GrpcEndpointConfiguration :
        EndpointConfiguration,
        IGrpcEndpointConfiguration
    {
        readonly IGrpcTopologyConfiguration _topologyConfiguration;

        protected GrpcEndpointConfiguration(IGrpcTopologyConfiguration topologyConfiguration)
            : base(topologyConfiguration)
        {
            _topologyConfiguration = topologyConfiguration;
        }

        GrpcEndpointConfiguration(IGrpcEndpointConfiguration parentConfiguration, IGrpcTopologyConfiguration topologyConfiguration)
            : base(parentConfiguration, topologyConfiguration)
        {
            _topologyConfiguration = topologyConfiguration;
        }

        IGrpcTopologyConfiguration IGrpcEndpointConfiguration.Topology => _topologyConfiguration;

        public IGrpcEndpointConfiguration CreateEndpointConfiguration()
        {
            var topologyConfiguration = new GrpcTopologyConfiguration(_topologyConfiguration);

            return new GrpcEndpointConfiguration(this, topologyConfiguration);
        }
    }
}
