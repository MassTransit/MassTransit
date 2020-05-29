namespace MassTransit.RabbitMqTransport.Topology
{
    using Builders;
    using GreenPipes;


    public interface IRabbitMqConsumeTopologySpecification :
        ISpecification
    {
        void Apply(IReceiveEndpointBrokerTopologyBuilder builder);
    }
}
