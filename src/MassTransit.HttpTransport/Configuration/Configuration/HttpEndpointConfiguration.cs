namespace MassTransit.HttpTransport.Configuration
{
    using MassTransit.Configuration;


    public class HttpEndpointConfiguration :
        EndpointConfiguration,
        IHttpEndpointConfiguration
    {
        readonly IHttpTopologyConfiguration _topologyConfiguration;

        protected HttpEndpointConfiguration(IHttpTopologyConfiguration topologyConfiguration)
            : base(topologyConfiguration)
        {
            _topologyConfiguration = topologyConfiguration;
        }

        HttpEndpointConfiguration(IHttpEndpointConfiguration parentConfiguration, IHttpTopologyConfiguration topologyConfiguration)
            : base(parentConfiguration, topologyConfiguration)
        {
            _topologyConfiguration = topologyConfiguration;
        }

        public new IHttpTopologyConfiguration Topology => _topologyConfiguration;

        public IHttpEndpointConfiguration CreateEndpointConfiguration()
        {
            var topologyConfiguration = new HttpTopologyConfiguration(_topologyConfiguration);

            return new HttpEndpointConfiguration(this, topologyConfiguration);
        }
    }
}
