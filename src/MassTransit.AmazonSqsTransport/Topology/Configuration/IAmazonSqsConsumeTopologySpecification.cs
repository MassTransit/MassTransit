namespace MassTransit.AmazonSqsTransport.Topology
{
    using Builders;
    using GreenPipes;


    public interface IAmazonSqsConsumeTopologySpecification :
        ISpecification
    {
        void Apply(IReceiveEndpointBrokerTopologyBuilder builder);
    }
}
