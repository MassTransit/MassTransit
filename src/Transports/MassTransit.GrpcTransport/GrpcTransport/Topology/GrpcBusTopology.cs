namespace MassTransit.GrpcTransport.Topology
{
    using Configuration;
    using Transports;


    public class GrpcBusTopology :
        BusTopology,
        IGrpcBusTopology
    {
        readonly IGrpcTopologyConfiguration _configuration;

        public GrpcBusTopology(IGrpcHostConfiguration hostConfiguration, IGrpcTopologyConfiguration configuration)
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