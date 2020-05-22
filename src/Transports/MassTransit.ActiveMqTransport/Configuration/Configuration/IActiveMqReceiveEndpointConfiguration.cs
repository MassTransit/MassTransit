namespace MassTransit.ActiveMqTransport.Configuration
{
    using MassTransit.Configuration;
    using Topology;


    public interface IActiveMqReceiveEndpointConfiguration :
        IReceiveEndpointConfiguration,
        IActiveMqEndpointConfiguration
    {
        ReceiveSettings Settings { get; }

        void Build(IHost host);
    }
}
