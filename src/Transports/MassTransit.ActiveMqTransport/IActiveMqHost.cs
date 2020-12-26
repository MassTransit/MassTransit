namespace MassTransit.ActiveMqTransport
{
    using Topology;


    public interface IActiveMqHost :
        IHost<IActiveMqReceiveEndpointConfigurator>
    {
        new IActiveMqHostTopology Topology { get; }
    }
}
