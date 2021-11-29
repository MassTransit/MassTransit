namespace MassTransit
{
    using ActiveMqTransport.Topology;


    public interface IActiveMqConsumeTopologySpecification :
        ISpecification
    {
        void Apply(IReceiveEndpointBrokerTopologyBuilder builder);
    }
}
