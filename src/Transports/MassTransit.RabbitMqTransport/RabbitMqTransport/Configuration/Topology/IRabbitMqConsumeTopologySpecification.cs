namespace MassTransit.RabbitMqTransport.Configuration
{
    using Topology;


    public interface IRabbitMqConsumeTopologySpecification :
        ISpecification
    {
        void Apply(IReceiveEndpointBrokerTopologyBuilder builder);
    }
}
