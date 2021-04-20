namespace MassTransit.GrpcTransport.Topology
{
    using MassTransit.Topology;


    public interface IGrpcConsumeTopology :
        IConsumeTopology
    {
        new IGrpcMessageConsumeTopology<T> GetMessageTopology<T>()
            where T : class;

        /// <summary>
        /// Apply the entire topology to the builder
        /// </summary>
        /// <param name="builder"></param>
        void Apply(GrpcTransport.Builders.IGrpcConsumeTopologyBuilder builder);
    }
}
