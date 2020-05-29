namespace MassTransit.ActiveMqTransport.Topology
{
    using Builders;
    using GreenPipes;


    public interface IActiveMqConsumeTopologySpecification :
        ISpecification
    {
        void Apply(IReceiveEndpointBrokerTopologyBuilder builder);
    }
}
