namespace MassTransit.RabbitMqTransport.Topology
{
    using GreenPipes;


    public interface IRabbitMqPublishTopologySpecification :
        ISpecification
    {
        void Apply(IRabbitMqPublishTopologyBuilder builder);
    }
}