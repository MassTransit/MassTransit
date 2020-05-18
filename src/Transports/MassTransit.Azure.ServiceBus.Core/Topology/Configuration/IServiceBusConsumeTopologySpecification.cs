namespace MassTransit.Azure.ServiceBus.Core.Topology
{
    using Builders;
    using GreenPipes;


    public interface IServiceBusConsumeTopologySpecification :
        ISpecification
    {
        void Apply(IReceiveEndpointBrokerTopologyBuilder builder);
    }
}
