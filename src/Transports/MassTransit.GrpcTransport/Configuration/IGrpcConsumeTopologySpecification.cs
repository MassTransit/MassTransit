namespace MassTransit
{
    using Configuration;


    public interface IGrpcConsumeTopologySpecification :
        ISpecification
    {
        void Apply(IMessageFabricConsumeTopologyBuilder builder);
    }
}
