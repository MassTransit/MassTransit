namespace MassTransit.ActiveMqTransport.Topology
{
    public interface IActiveMqBindingConsumeTopologySpecification :
        IActiveMqConsumeTopologySpecification,
        ITopicBindingConfigurator
    {

    }
}
