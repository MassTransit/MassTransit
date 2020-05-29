namespace MassTransit.RabbitMqTransport.Topology
{
    using Builders;
    using GreenPipes;


    public interface IRabbitMqPublishTopologySpecification :
        ISpecification
    {
        void Apply(IPublishEndpointBrokerTopologyBuilder builder);
    }
}
