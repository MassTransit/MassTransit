namespace MassTransit.GrpcTransport.Topology.Topologies
{
    using Configuration;
    using MassTransit.Topology.Topologies;


    public class GrpcHostTopology :
        HostTopology,
        IGrpcHostTopology
    {
        readonly IGrpcTopologyConfiguration _configuration;

        public GrpcHostTopology(IGrpcHostConfiguration hostConfiguration, IGrpcTopologyConfiguration configuration)
            : base(hostConfiguration, configuration)
        {
            _configuration = configuration;
        }

        public new IGrpcMessagePublishTopology<T> Publish<T>()
            where T : class
        {
            return _configuration.Publish.GetMessageTopology<T>();
        }
    }
}