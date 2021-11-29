namespace MassTransit
{
    using GrpcTransport.Configuration;


    public interface IGrpcConsumeTopologySpecification :
        ISpecification
    {
        void Apply(IGrpcConsumeTopologyBuilder builder);
    }
}
