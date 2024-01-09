namespace MassTransit.SqlTransport.Configuration
{
    using Topology;


    public interface ISqlPublishTopologySpecification :
        ISpecification
    {
        void Apply(IPublishEndpointBrokerTopologyBuilder builder);
    }
}
