namespace MassTransit.GrpcTransport.Topology.Configurators
{
    using MassTransit.Topology;


    public interface IGrpcConsumeTopologyConfigurator :
        IConsumeTopologyConfigurator,
        IGrpcConsumeTopology
    {
        new IGrpcMessageConsumeTopologyConfigurator<T> GetMessageTopology<T>()
            where T : class;

        void AddSpecification(IGrpcConsumeTopologySpecification specification);
    }
}
