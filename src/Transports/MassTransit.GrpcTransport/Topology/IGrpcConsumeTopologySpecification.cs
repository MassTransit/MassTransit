namespace MassTransit.GrpcTransport.Topology
{
    using GreenPipes;
    using GrpcTransport.Builders;


    public interface IGrpcConsumeTopologySpecification :
        ISpecification
    {
        void Apply(IGrpcConsumeTopologyBuilder builder);
    }
}